//
//  SampleVisitor.cpp
//  SampleTranslator
//
//  Created by Rustam on 04/06/2015.
//  Copyright (c) 2015 Rustam. All rights reserved.
//

#include <string>
#include <stdio.h>
#include <iostream>
#include <sstream>

#include "clang/AST/RecursiveASTVisitor.h"
#include "clang/AST/ASTContext.h"
#include "clang/AST/DeclCXX.h"
#include "clang/AST/DeclObjC.h"
#include "clang/Lex/Lexer.h"
//#include "clang/AST/"
#include "clang/Frontend/CompilerInstance.h"
#include "clang/Rewrite/Core/Rewriter.h"
#include "SampleVisitor.h"

#include "llvm/Support/raw_ostream.h"

using namespace clang;
using namespace std;

SampleVisitor::SampleVisitor(CompilerInstance &CI, StringRef currentFile, map<string, string> signatures)
    : compiler (CI), sourceManager(CI.getSourceManager()), file(currentFile), defaultSignatures(signatures)
{
    for (map<string, string>::iterator it = defaultSignatures.begin(); it != defaultSignatures.end(); ++it) {
        cout << it->first << "\n";
        cout << it->second << "\n";
    }
}

bool SampleVisitor::VisitCXXRecordDecl(CXXRecordDecl *Declaration)
{
    Declaration->dump();
    return true;
}

bool SampleVisitor::VisitObjCInterfaceDecl(ObjCInterfaceDecl *D)
{
//    D->dump();
    if(D-> getNameAsString() == "ViewController") {
//        D->dump();
    }
    return true;
}


bool SampleVisitor::VisitObjCImplDecl(ObjCImplDecl *D)
{
    if(ObjCImplementationDecl * implDecl = dyn_cast<ObjCImplementationDecl>(D)) {
        if(ObjCInterfaceDecl *interfaceDecl = implDecl-> getClassInterface()) {
            PrintClassName(interfaceDecl);
            
            for(clang::CapturedDecl::specific_decl_iterator<ObjCMethodDecl> m = D->meth_begin(); m != D->meth_end(); ++m) {
                ObjCMethodDecl* methodDecl = (*m);
                PrintMethod(methodDecl);
                llvm::outs() << "\n\n";
            }
        }
    }
//    D->dump();
    return true;
}

bool SampleVisitor::IsFromCurrentFile(clang::SourceLocation location)
{
    return sourceManager.getFilename(location) == file;
}

void SampleVisitor::PrintClassName(ObjCInterfaceDecl * interfaceDecl)
{
    llvm::outs() << "public class " << interfaceDecl->getNameAsString();
    if(auto super = interfaceDecl->getSuperClass())
        llvm::outs() << " : " << super-> getNameAsString() << "\n";
}

void SampleVisitor::PrintMethod(ObjCMethodDecl *methodDecl)
{
    if (!IsFromCurrentFile(methodDecl->getLocation()))
        return;

    Selector selector = methodDecl->getSelector();
    auto returnType = GetPointeeName(methodDecl->getReturnType());
    
    string stringSelector = selector.getAsString();
    auto iter = defaultSignatures.find(stringSelector);
    if(iter != defaultSignatures.end())
        llvm::outs() << iter->second;
    else
        llvm::outs() << "public " << returnType << "  " << selector.getAsString();
    PrintMethodParams(methodDecl);
    llvm::outs() << "\n";

    llvm::outs() << "{\n";
    string methodBodyText = CommentSrc(GetBodyText(methodDecl));
    llvm::outs() << methodBodyText;
    llvm::outs() << "}";
}

void SampleVisitor::PrintMethodParams(ObjCMethodDecl *methodDecl)
{
    llvm::outs() << "(";
    bool isFirst = true;
    for (auto param = methodDecl->param_begin(); param != methodDecl->param_end(); ++param, isFirst = false) {
        QualType paramType = (*param)->getType();
        string paramName = (*param)->getNameAsString();
        llvm::outs() << (isFirst ? "": ", ") << GetPointeeName(paramType) << " " << paramName;
    }
    llvm::outs() << ")";
}

string SampleVisitor::GetBodyText(ObjCMethodDecl *methodDecl)
{
    Stmt *methodBody = methodDecl->getBody();
    SourceRange bodyRange;
    if(CompoundStmt *stmt = dyn_cast<CompoundStmt>(methodBody))
        bodyRange = stmt->body_back() -> getSourceRange();
    else
        bodyRange = methodBody->getSourceRange();
    
    Rewriter rewriter;
    rewriter.setSourceMgr(sourceManager, compiler.getLangOpts());
    return rewriter.getRewrittenText(bodyRange);
}

string SampleVisitor::CommentSrc(string src)
{
    string line;
    string output;
    istringstream stream (src);
    while(getline(stream, line)) {
        output = output + "//" + line + "\n";
    }
    
    return output;    
}

string SampleVisitor::GetPointeeName(QualType qualType)
{
    if(const ObjCObjectPointerType * objcPointerType = qualType.getTypePtr()->getAsObjCInterfacePointerType()) {
        if(const ObjCInterfaceType * interfaceType = objcPointerType->getInterfaceType())
            return interfaceType->getDecl()->getNameAsString();
    }

    return qualType.getAsString();
}





















