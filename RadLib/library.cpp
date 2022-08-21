#include "library.h"
#include "dll.c"
#include <iostream>

extern "C" {
DLL_PUBLIC void hello()
{
    std::cout << "Hello, World!!!" << std::endl;
}
}
