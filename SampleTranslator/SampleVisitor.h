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
#include "clang/Basic/SourceManager.h"
#include "clang/AST/DeclCXX.h"
#include "clang/AST/DeclObjC.h"
#include "clang/Frontend/CompilerInstance.h"

#include <stdio.h>
#include <string>

using namespace clang;
using namespace std;


class SampleVisitor : public RecursiveASTVisitor<SampleVisitor>
{
private:
    CompilerInstance &compiler; // used for getting additional AST info
    SourceManager &sourceManager;
    map<string, string> defaultSignatures;
    StringRef file;
    bool IsFromCurrentFile (SourceLocation location);
    void PrintMethod (ObjCMethodDecl *methodDecl);
    
    
public:
    SampleVisitor(CompilerInstance &CI, StringRef file, map<string, string> signatures);
    bool VisitCXXRecordDecl(CXXRecordDecl *Declaration);
    bool VisitObjCImplDecl(ObjCImplDecl *D);
    bool VisitObjCInterfaceDecl(ObjCInterfaceDecl *D);
};

#endif /* defined(__SampleTranslator__SampleVisitor__) */
