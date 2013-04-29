#-----------------------------------------------------------------------------------------------------------------------------------------------------------------------#
# 										Copyright 2005 Micron Technology, Inc. All Rights Reserved.
# 													UK Design Centre, Bracknell
#-----------------------------------------------------------------------------------------------------------------------------------------------------------------------#
# MIDLib.pm
# 
# Purpose: Wrapper for Micron COM library for supporting imaging devices. Provides convenient access to various commands.
#
# $Log: MIDLib.pm.rca $
# 
#  Revision: 1.3 Tue Mar 15 15:24:59 2005 bcorr
#  alpha release
#
#-----------------------------------------------------------------------------------------------------------------------------------------------------------------------#
package MIDLib;
use File::Find;
use Exporter;
use Win32::OLE;

use strict;

BEGIN { our $VERSION = $1 if ('$Revision: 1.3 $ ' =~ /\s+([0-9.]+)\s+/); }

our @ISA = ('Exporter', 'Win32::OLE');
our @EXPORT = qw(	
	setMode
	getMode
	);

our @EXPORT_OK = qw(	
	setMode
	getMode
	);

my %Enum;

# Delegates the object creation back to the Win32::OLE class, but adds a call to get all the mode data from the midlib.h file
sub new 
{
	# use the top level new method, but also initialise the mode data from the installed header files
	my $self = $_[0]->SUPER::new(@_[1..$#_]);
	&init_mode_data;

	return $self;
}

# looks for midlib.h file and reads this file to get enumeration data for mi_modes which is used in the set/getMode method replacements
sub init_mode_data
{
	# search for midlib.h in the Micron install directory
	my $path = $ENV{MI_HOME};

	find(\&read_header, $path);
}

# parses header file to extract mi_modes data, which is put in the %Enum HoH
sub read_header
{
	return unless ($_ eq 'midlib.h');
	my $header_file = $File::Find::name;
	print "Reading $header_file\n";
	
	unless (open HEADER, "< $header_file") {
		print "*** Error : Could not open midlib header file, the setMode method will not work\n";
		return;
	}
	
	my $enum;
	my $buffer = '';
	my $in_comment;
	
	while (<HEADER>) {
		chomp;
		$enum = 1 if /^\s*typedef\s+enum/;		# flag to locate enum blocks

		$in_comment = 1 if /\/\*/;				# skip /*.. */ comment blocks
		if (/\*\/\s*$/) {
			$in_comment = 0;
			next;
		}
		next if $in_comment;

		s/\/\/.*//;								# strip // comments
		$buffer .= $_ if $enum;

		if (/}/) {								# process enum block
			$enum = 0;
			$buffer =~ s/[{};]/ /g;
			# $buffer looks like : typedef enum  MI_REG_ADDR  MI_MCU_ADDR  mi_addr_type
			$buffer =~ s/typedef\s+enum//;
			$buffer =~ s/\s+/ /g;
			my @buffer = split " ", $buffer;	# split on spaces to get the last field
			my $type = pop @buffer;
			$type =~ s/\s+//g;
			@buffer = split /,/, join " ", @buffer;		# and then split on commas to get the types

			# 
			# @buffer now contains 
			for (my $i = 0; $i <= $#buffer; $i++) {
				if ($buffer[$i] =~ /=/) {
					my ($key, $value) = split /=/, $buffer[$i], 2;
					$key =~ s/\s+//g;
					$value =~ s/\s+//g;
					$Enum{$type}{$key} = $value;
				}
				else { 
					$buffer[$i] =~ s/\s+//g;
					$Enum{$type}{$buffer[$i]} = $i;
				}
			}
			$buffer = '';
		}
	}
}

#-----------------------------------------------------------------------------------------------------------------------------------------------------------------------#

# looks up the magic number that corresponds to the mode text and passes it to the MIDLibCOM method for handling (via Win32::OLE)
sub setMode
{
	my ($obj, $mode, $value) = @_;
	my $index = $Enum{mi_modes}{$mode};
	unless (defined $index) {
		print "Error : no such mode ($mode)\n";
		return;
	}
	$obj->SUPER::setMode($index, $value);
}

sub getMode
{
	my ($obj, $mode) = @_;
	my $index = $Enum{mi_modes}{$mode};
	unless (defined $index) {
		print "Error : no such mode ($mode)\n";
		return undef;
	}
	return $obj->SUPER::getMode($index);

}

sub listModes
{
	my $obj = shift;
	print join "\n", sort keys %{$Enum{mi_modes}};
}

#-----------------------------------------------------------------------------------------------------------------------------------------------------------------------#

sub readRegister
{
	my ($obj, $ship_base_add, $address) = @_;
	my $value = $obj->SUPER::readRegister($ship_base_add, $address);

	#print "[$ship_base_add] [$address] reading $value\n";
	$value >>= 8 if ($obj->getMode('MI_REG_DATA_SIZE') == 8);
	return $value;
}

sub writeRegister
{
	my ($obj, $ship_base_add, $address, $value) = @_;

	# FIXME this is a bodge to get 8 bit write to work correctly until firmware upgrade is done
	# 8 bit read of addr 0x00 returns (0x00, 0x01)
	# So to do an 8 bit write we shift data one byte to right and OR it with existing data
	if ($obj->getMode('MI_REG_DATA_SIZE') == 8) {
		$obj->setMode('MI_REG_DATA_SIZE', 16);

		my $current_value = $obj->readRegister($ship_base_add, $address);

		# chuck out MSB of current value
		$current_value &= 0xff;	

		# insert new data into MSB
		my $new_value = $current_value | ($value << 8);
		$obj->SUPER::writeRegister($ship_base_add, $address, $new_value);

		$obj->setMode('MI_REG_DATA_SIZE', 8);
	}
	else {
		$obj->SUPER::writeRegister($ship_base_add, $address, $value);
	}
	#print "[$ship_base_add] [$address] writing $value\n";
}

sub orRegister
{
	my ($obj, $ship_base_add, $address, $value) = @_;
	my $current_value = $obj->readRegister($ship_base_add, $address);
	my $new_value = $current_value | $value;
	$obj->writeRegister($ship_base_add, $address, $new_value);
}

sub andRegister
{
	my ($obj, $ship_base_add, $address, $value) = @_;
	my $current_value = $obj->readRegister($ship_base_add, $address);
	my $new_value = $current_value & $value;
	$obj->writeRegister($ship_base_add, $address, $new_value);
}

#-----------------------------------------------------------------------------------------------------------------------------------------------------------------------#
# takes a photo and saves the frame to given filename
sub saveFrame
{
	my ($Camera, $filename, $width, $height, $bits) = @_;
	$bits ||= 8;

	print "Saving a ($width, $height) $bits bits/pixel frame to $filename\n";
	$Camera->updateFrameSize($width, $height, $bits, 1);
	#my $Sensor = $Camera->sensor;
	# get buffer size
	#my $width_  = $Sensor->width;
	#my $height_ = $Sensor->height;

	#print "$width_ : $height_\n";	
	my @frame = $Camera->grabFrame();

	my $actual_width  = @{$frame[0]};
	my $actual_height = @{$frame[0][0]};
	my $pixel_size = length (sprintf "%b", $frame[0][0][0]);
	print "Pixel length = $pixel_size bits\n";

	unless (($actual_width == $width) and ($actual_height == $height)) {
		print "*** Warning : Captured frame size doesn't match specified frame size\n";
	}
	
	unlink $filename;

	unless (open RAW, "> $filename") {
		print "Couldn't open $filename for writing\n";
		return;
	}

	binmode RAW;

	for (my $y = 0; $y <= $height-1; $y++) {
		for (my $x = 0; $x <= $width-1; $x++) {
			my $data = $frame[0][$x][$y];
			my $fdata = pack("v", $data);
			print RAW $fdata;
		}
	}

	close RAW;
}

sub Hex2Chr
{
	sprintf "%x", shift;
}

1;
