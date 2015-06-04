//
//  FindClassesAction.h
//  SampleTranslator
//
//  Created by Rustam on 04/06/2015.
//  Copyright (c) 2015 Rustam. All rights reserved.
//

#ifndef __SampleTranslator__FindClassesAction__
#define __SampleTranslator__FindClassesAction__

#include <stdio.h>

#include "clang/AST/ASTConsumer.h"
#include "clang/Frontend/FrontendAction.h"
#include "clang/Frontend/CompilerInstance.h"

using namespace std;
using namespace clang;

class FindClassesAction : public clang::ASTFrontendAction {
public:
    FindClassesAction() = default;
    virtual unique_ptr<ASTConsumer> CreateASTConsumer(CompilerInstance &Compiler, llvm::StringRef InFile);
};

#endif /* defined(__SampleTranslator__FindClassesAction__) */
