//
//  FindClassesAction.cpp
//  SampleTranslator
//
//  Created by Rustam on 04/06/2015.
//  Copyright (c) 2015 Rustam. All rights reserved.
//

#include "clang/AST/ASTConsumer.h"
#include "clang/Frontend/CompilerInstance.h"

#include "FindClassesAction.h"
#include "SampleASTConsumer.h"

#include <iostream>
#include <fstream>
#include <string>

using namespace std;

FindClassesAction::FindClassesAction(string ns, string outputDir)
:projectNamespace(ns), outputDir(outputDir)
{
    string selector, signature;
    ifstream signaturesStorage;
    signaturesStorage.open ("signatures.txt");
    
    while (getline(signaturesStorage, selector) && getline(signaturesStorage, signature))
        defaultSignatures[selector] = signature;
    signaturesStorage.close();
}

unique_ptr<ASTConsumer> FindClassesAction::CreateASTConsumer(CompilerInstance &Compiler, llvm::StringRef InFile)
{
    llvm::outs() << InFile << "\n";
    return unique_ptr<ASTConsumer>(new SampleASTConsumer(Compiler, InFile, defaultSignatures, projectNamespace, outputDir));
}