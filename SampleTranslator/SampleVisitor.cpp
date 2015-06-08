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
#include "clang/AST/DeclObjC.h"
#include "clang/Lex/Lexer.h"
//#include "clang/AST/"
#include "clang/Frontend/CompilerInstance.h"
#include "clang/Rewrite/Core/Rewriter.h"
#include "SampleVisitor.h"

#include "llvm/Support/raw_ostream.h"

using namespace clang;

SampleVisitor::SampleVisitor(CompilerInstance &CI, StringRef currentFile)
    : compiler (CI), sourceManager(CI.getSourceManager()), file(currentFile)
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
            if(auto super = interfaceDecl->getSuperClass()) {
                llvm::outs() << super-> getNameAsString() << "\n";
            }
        }
        
        llvm::outs () << D->getNameAsString() << "\n\n";
        if( auto superClass = implDecl->getSuperClass()) {
            llvm::outs () << superClass->getName() << "\n";
        } else {
            llvm::outs () << "no supper\n";
        }
        
    }
//    D->dump();
    return true;
}

bool SampleVisitor::VisitObjCMethodDecl(ObjCMethodDecl *methodDecl)
{
    if (!IsFromCurrentFile(methodDecl->getLocation()))
        return true;
//    SourceLocation location = methodDecl->getLocation();
//    llvm::outs() << sourceManager.getFilename(location) << "\n";
//
    Selector selector = methodDecl->getSelector();
    auto returnType = methodDecl->getReturnType().getAsString();

    llvm::outs() << returnType << "  " << selector.getAsString() << "\n";
    
    for (auto param = methodDecl->param_begin(); param != methodDecl->param_end(); ++param) {
            QualType paramType = (*param)->getOriginalType();
            llvm::outs() << "(" << paramType.getAsString() << ") ";
    }
    llvm::outs() << "\n";
    
    Stmt *methodBody = methodDecl->getBody();
    SourceRange bodyRange;
    if(CompoundStmt *stmt = dyn_cast<CompoundStmt>(methodBody)) {
        bodyRange = stmt->body_back() -> getSourceRange();
    } else {
        bodyRange = methodBody->getSourceRange();
    }
    
    Rewriter rewriter;
    rewriter.setSourceMgr(sourceManager, compiler.getLangOpts());
    auto methodBodyText = rewriter.getRewrittenText(bodyRange);
    llvm::outs() << methodBodyText << "\n\n";

    return true;
}

bool SampleVisitor::IsFromCurrentFile(clang::SourceLocation location)
{
    return sourceManager.getFilename(location) == file;
}























