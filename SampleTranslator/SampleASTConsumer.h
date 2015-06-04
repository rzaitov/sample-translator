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
#include "clang/AST/ASTConsumer.h"
#include "clang/Frontend/CompilerInstance.h"
#include "SampleASTConsumer.h"
#include "SampleVisitor.h"

using namespace clang;

class SampleASTConsumer : public ASTConsumer {
private:
    SampleVisitor *visitor;
    
public:
    SampleASTConsumer(CompilerInstance &CI);
    
    // override this to call our ExampleVisitor on each top-level Decl
    virtual void HandleTranslationUnit(ASTContext &Context);
};

#endif /* defined(__SampleTranslator__SampleASTConsumer__) */
