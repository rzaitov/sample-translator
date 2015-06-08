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
#include "Writer.h"

#include "llvm/Support/raw_ostream.h"

using namespace clang;
using namespace std;

SampleVisitor::SampleVisitor(CompilerInstance &CI, StringRef currentFile, map<string, string> signatures, Writer *writer)
    : compiler (CI), sourceManager(CI.getSourceManager()), file(currentFile), defaultSignatures(signatures), writer(writer)
{
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
            writer->Indent();
            writer->Outs() << "\n{\n";
            writer->PushIndent();
            
            for(clang::CapturedDecl::specific_decl_iterator<ObjCMethodDecl> m = D->meth_begin(); m != D->meth_end(); ++m) {
                ObjCMethodDecl* methodDecl = (*m);
                PrintMethod(methodDecl);
                writer->Outs() << "\n\n";
            }
            
            writer->PopIndent();
            writer->Outs() << "}\n";
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
    writer->Indent();
    writer->Outs() << "public class " << interfaceDecl->getNameAsString();
    if(auto super = interfaceDecl->getSuperClass())
        writer->Outs() << " : " << super-> getNameAsString();
}

void SampleVisitor::PrintMethod(ObjCMethodDecl *methodDecl)
{
    if (!IsFromCurrentFile(methodDecl->getLocation()))
        return;

    Selector selector = methodDecl->getSelector();
    auto returnType = GetPointeeName(methodDecl->getReturnType());

    writer->Indent();
    string stringSelector = selector.getAsString();
    auto iter = defaultSignatures.find(stringSelector);
    if(iter != defaultSignatures.end())
        writer->Outs() << iter->second;
    else
        writer->Outs() << "public " << returnType << " " << ConvertSelectorToName(stringSelector);
    PrintMethodParams(methodDecl);
    writer->Outs() << "\n";

    writer->Indent();
    writer->Outs() << "{\n";
    writer-> PushIndent();
    PrintMethodBody(GetBodyText(methodDecl));
    writer-> PopIndent();
    writer->Indent();
    writer->Outs() << "}";
}

void SampleVisitor::PrintMethodParams(ObjCMethodDecl *methodDecl)
{
    writer->Outs() << " (";
    bool isFirst = true;
    for (auto param = methodDecl->param_begin(); param != methodDecl->param_end(); ++param, isFirst = false) {
        QualType paramType = (*param)->getType();
        string paramName = (*param)->getNameAsString();
        writer->Outs() << (isFirst ? "": ", ") << GetPointeeName(paramType) << " " << paramName;
    }
    writer->Outs() << ")";
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

void SampleVisitor::PrintMethodBody(string src)
{
    string line;
    string output;
    istringstream stream (src);
    while(getline(stream, line)) {
        writer -> Outs() << "//";
        writer -> Indent();
        writer -> Outs() << line << '\n';
    }
}

string SampleVisitor::GetPointeeName(QualType qualType)
{
    if(const ObjCObjectPointerType * objcPointerType = qualType.getTypePtr()->getAsObjCInterfacePointerType()) {
        if(const ObjCInterfaceType * interfaceType = objcPointerType->getInterfaceType())
            return interfaceType->getDecl()->getNameAsString();
    }

    return qualType.getAsString();
}

string SampleVisitor::ConvertSelectorToName (string selector)
{
    string line;
    string output;
    istringstream stream (selector);
    while(getline(stream, line, ':')) {
        line[0] = toupper(line[0]);
        output += line;
    }
    
    return output;
}
