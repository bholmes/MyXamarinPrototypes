//
//  TestClass.m
//  MatrixTest
//
//  Created by Bill Holmes on 10/17/13.
//  Copyright (c) 2013 Xamarin. All rights reserved.
//

#import "TestClass.h"
#include <sys/time.h>
#include <Accelerate/Accelerate.h>

static void mallocOnDevice(size_t numBytes, void** memPtr)
{
    *memPtr = malloc(numBytes);
}

static void freeOnDevice(void* memPtr)
{
    free(memPtr);
}

static void matrixMultBLAS (int n, double *A, double *B, double *C)
{
    double scale = 1;
    double scale2 = 0;
    cblas_dgemm (CblasRowMajor, CblasNoTrans, CblasNoTrans,
                 n, n, n,scale, A, n, B, n, scale2, C, n);
}

@implementation TestClass

@synthesize matrixSize;

-(id) init
{
    id ret = [super init];
    
    self.matrixSize = 256;
    
    return ret;
}

-(void) runTest
{
    int i;
    int n = self.matrixSize;
    size_t numBytes;
    double *aMat, *bMat, *cMat, *dMat;
    
    numBytes = n*n*sizeof(double);
    mallocOnDevice(numBytes, (void**)&aMat);
    mallocOnDevice(numBytes, (void**)&bMat);
    mallocOnDevice(numBytes, (void**)&cMat);
    mallocOnDevice(numBytes, (void**)&dMat);
    
    for (i=0; i<n*n; i++) {
        aMat[i] = rand() / (double)RAND_MAX;
    }
    for (i=0; i<n*n; i++) {
        bMat[i] = rand() / (double)RAND_MAX;
    }
    
    struct timeval multStart;
    gettimeofday(&multStart, NULL);
    
    matrixMultBLAS(n, aMat, bMat, cMat);
    
    struct timeval multEnd;
    gettimeofday(&multEnd, NULL);
    
    freeOnDevice(aMat);
    freeOnDevice(bMat);
    freeOnDevice(cMat);
    freeOnDevice(dMat);
    
    long elapsed_seconds  = multEnd.tv_sec  - multStart.tv_sec;
    long elapsed_useconds = multEnd.tv_usec - multStart.tv_usec;
    
    self.matrixMultiplyTime = ((double)elapsed_seconds) + (((double)elapsed_useconds)/1000000);
    
}

@end



