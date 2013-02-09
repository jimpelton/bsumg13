#ifndef DBG_PRINT_H
#define DBG_PRINT_H

#ifdef DBG_PRINT
#define dbg_print(_str, ...) fprintf(stderr, "%d:%s()::" # _str,__LINE__, __func__, __VA_ARGS__)
#else
#define dbg_print(_str, ...)
#endif  /* NDEBUG */

bool uG_DEBUG_CIRCLES=false;


#endif // DBG_PRINT_H
