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
//#include "clang/AST/"
#include "clang/Frontend/CompilerInstance.h"
#include "SampleVisitor.h"

#include "llvm/Support/raw_ostream.h"

using namespace clang;

SampleVisitor::SampleVisitor(CompilerInstance &CI, StringRef currentFile)
    : astContext (CI.getASTContext()), sourceManager(CI.getSourceManager()), file(currentFile)
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
    llvm::outs() << selector.getAsString() << "\n";

    return true;
}

bool SampleVisitor::IsFromCurrentFile(clang::SourceLocation location)
{
    return sourceManager.getFilename(location) == file;
}























