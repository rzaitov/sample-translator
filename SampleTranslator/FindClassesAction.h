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
#include <string>

#include "clang/AST/ASTConsumer.h"
#include "clang/Frontend/FrontendAction.h"
#include "clang/Frontend/CompilerInstance.h"
#include "clang/Tooling/Tooling.h"

using namespace std;
using namespace clang;

class FindClassesAction : public clang::ASTFrontendAction {
private:
    map<string, string> defaultSignatures;
    string projectNamespace;
    
public:
    FindClassesAction(string ns);
    virtual unique_ptr<ASTConsumer> CreateASTConsumer(CompilerInstance &Compiler, llvm::StringRef InFile);
};

class ActionFactory : public clang::tooling::FrontendActionFactory {
private:
    string ns;
    
public:
    ActionFactory(string ns)
    : ns(ns)
    { }
    
    clang::FrontendAction *create() override { return new FindClassesAction(ns); }
};

#endif /* defined(__SampleTranslator__FindClassesAction__) */
