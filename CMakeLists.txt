set(LLVM_LINK_COMPONENTS support)
set(LLVM_USED_LIBS clangTooling clangBasic clangAST)

add_clang_executable(sample-translator
  SampleTranslator/main.cpp
  SampleTranslator/SampleTranslator.cpp
  SampleTranslator/SampleASTConsumer.cpp
  SampleTranslator/SampleVisitor.cpp
  SampleTranslator/FindClassesAction.cpp
  SampleTranslator/Writer.cpp
  )
target_link_libraries(sample-translator
  clangTooling
  clangBasic
  clangASTMatchers
  )