//
//  SampleVisitor.cpp
//  SampleTranslator
//
//  Created by Rustam on 04/06/2015.
//  Copyright (c) 2015 Rustam. All rights reserved.
//

#include "clang/AST/RecursiveASTVisitor.h"
#include "clang/AST/ASTContext.h"
#include "clang/AST/DeclCXX.h"
#include "clang/Frontend/CompilerInstance.h"
#include "SampleVisitor.h"

using namespace clang;

SampleVisitor::SampleVisitor(CompilerInstance &CI)
    : astContext (CI.getASTContext())
{
        
}

bool SampleVisitor::VisitCXXRecordDecl(CXXRecordDecl *Declaration)
{
    Declaration->dump();
    return true;
}
