//
//  SampleASTConsumer.cpp
//  SampleTranslator
//
//  Created by Rustam on 04/06/2015.
//  Copyright (c) 2015 Rustam. All rights reserved.
//

#include "clang/AST/ASTConsumer.h"
#include "clang/AST/ASTContext.h"
#include "clang/Frontend/CompilerInstance.h"
#include "SampleASTConsumer.h"
#include "SampleVisitor.h"

using namespace clang;

SampleASTConsumer::SampleASTConsumer(CompilerInstance &CI)
    : visitor(new SampleVisitor(CI)) // initialize the visitor
{
}
    
    // override this to call our ExampleVisitor on each top-level Decl
void SampleASTConsumer::HandleTranslationUnit(clang::ASTContext &ctx)
{
    /* we can use ASTContext to get the TranslationUnitDecl, which is
    a single Decl that collectively represents the entire source file */
   
    auto unitDecl = ctx.getTranslationUnitDecl ();
    visitor->TraverseDecl(unitDecl);
}