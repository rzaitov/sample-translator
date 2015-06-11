//
//  SampleASTConsumer.h
//  SampleTranslator
//
//  Created by Rustam on 04/06/2015.
//  Copyright (c) 2015 Rustam. All rights reserved.
//

#ifndef __SampleTranslator__SampleASTConsumer__
#define __SampleTranslator__SampleASTConsumer__

#include <stdio.h>
#include <string>

#include "clang/AST/ASTContext.h"
#include "clang/Frontend/CompilerInstance.h"

#include "SampleVisitor.h"

using namespace clang;

class SampleASTConsumer : public ASTConsumer {
private:
    SampleVisitor *visitor;
    Writer *writer;
    std::map<string, string> defaultSignatures;
    string projectNamespace;
    string csFile;
    
public:
    SampleASTConsumer(CompilerInstance &CI, StringRef inFile, std::map<string, string> signatures, string projectNamespace, string outputDir);
    
    // override this to call our ExampleVisitor on each top-level Decl
    virtual void HandleTranslationUnit(ASTContext &Context);
    
    virtual void HandleImplicitImportDecl(ImportDecl *D);
};

#endif /* defined(__SampleTranslator__SampleASTConsumer__) */
