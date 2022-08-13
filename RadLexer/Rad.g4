parser grammar Rad;

options { tokenVocab='RadLexer\\RadLexer'; }

// Top Level
startRule: topLevel;
topLevel: (definiteStatement | declaration)* EOF;
declaration: functionDeclaration;

//Functions
// Function Declaration
functionDeclaration: FN ID namedTypeTuple returnTypeSpecifier THEN functionBody;
returnTypeSpecifier: typeSpecifier | voidSpecifier;
typeSpecifier: COLON (numberType | ID);
voidSpecifier: COLON VOID;
functionBody: statementGroup; 
// Function Call
functionCall: ID orderedTuple;
 
// Tuples
// Named Tuples
namedTypeTuple: LPAREN namedParameters RPAREN;
namedParameters: namedParameter (COMMA namedParameter)*; 
namedParameter: ID typeSpecifier;
// Ordered Tuples
orderedTuple: LPAREN orderedParameters RPAREN;
orderedParameters: orderedParameter (COMMA orderedParameter)*;
orderedParameter: ID | literal;

// Grouping
statementGroup: LCURL definiteStatement* RCURL;

// Type Literals
numberType: UNSIGNED? keyword=(INT_KEYWORD | FLOAT_KEYWORD);

// Statements and Expressions
statementKeyword: RETURN | OUT;
definiteStatement: statementKeyword? expression? SEMICOLON;
expression: expression op=(STAR | FSLASH) expression
    | expression op=(PLUS | MINUS) expression
    | literal
    | functionCall
    | ID;

// Operators
binaryOperator: STAR | PLUS | MINUS | FSLASH;

// Literal
literal: numericLiteral | stringLiteral;
numericLiteral: NUMBER FLAG?;
stringLiteral: ((DQUOTE.*?DQUOTE) | (SQUOTE.*?SQUOTE))FLAG?;
