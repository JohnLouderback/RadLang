cmake_minimum_required(VERSION 3.23)
project(RadLib)

set(CMAKE_CXX_STANDARD 11)

add_definitions(-DBUILDING_DLL)

add_library(RadLib SHARED library.cpp dll.c)
add_library(RadLibStatic STATIC library.cpp dll.c)
