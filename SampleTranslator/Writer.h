//
//  Writer.h
//  SampleTranslator
//
//  Created by Rustam on 08/06/2015.
//  Copyright (c) 2015 Rustam. All rights reserved.
//

#ifndef __SampleTranslator__Writer__
#define __SampleTranslator__Writer__

#include <stdio.h>
#include <string>
#include <sstream>

using namespace std;

class Writer
{
private:
    ostringstream writer;
    int indent;
    
public:
    Writer ();
    void PushIndent ();
    void PopIndent ();
    void Indent ();
    ostringstream& Outs();
};

#endif /* defined(__SampleTranslator__Writer__) */
