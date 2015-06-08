//
//  Writer.cpp
//  SampleTranslator
//
//  Created by Rustam on 08/06/2015.
//  Copyright (c) 2015 Rustam. All rights reserved.
//

#include <stdio.h>
#include <string>
#include <sstream>

#include "Writer.h"

using namespace std;

Writer::Writer ()
{
    indent = 0;
}

void Writer::Indent()
{
    for (int i = 0; i < indent; ++i)
        writer << '\t';
}

void Writer::PopIndent()
{
    indent--;
}

void Writer::PushIndent()
{
    indent++;
}

ostringstream& Writer::Outs()
{
    return writer;
}