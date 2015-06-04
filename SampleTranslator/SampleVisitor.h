//
//  SampleVisitor.h
//  SampleTranslator
//
//  Created by Rustam on 04/06/2015.
//  Copyright (c) 2015 Rustam. All rights reserved.
//

#ifndef __SampleTranslator__SampleVisitor__
#define __SampleTranslator__SampleVisitor__

#include "clang/AST/RecursiveASTVisitor.h"
#include "clang/AST/ASTContext.h"
#include "clang/AST/DeclCXX.h"
#include "clang/Frontend/CompilerInstance.h"

#include <stdio.h>

using namespace clang;

class SampleVisitor : public RecursiveASTVisitor<SampleVisitor>
{
private:
    ASTContext &astContext; // used for getting additional AST info
    
public:
    SampleVisitor(CompilerInstance &CI);
    bool VisitCXXRecordDecl(CXXRecordDecl *Declaration);
    
    
};

#endif /* defined(__SampleTranslator__SampleVisitor__) */
