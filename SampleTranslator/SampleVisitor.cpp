//
//  SampleVisitor.cpp
//  SampleTranslator
//
//  Created by Rustam on 04/06/2015.
//  Copyright (c) 2015 Rustam. All rights reserved.
//

#include "clang/AST/RecursiveASTVisitor.h"
#include "clang/Frontend/CompilerInstance.h"
#include "SampleVisitor.h"

using namespace clang;

SampleVisitor::SampleVisitor(CompilerInstance &CI)
    : astContext (CI.getASTContext())
{
        
}
