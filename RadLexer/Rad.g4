parser grammar Rad;

options {
	tokenVocab = 'RadLexer\\RadLexer';
}

// Top Level
startRule: topLevel;
topLevel: (definiteStatement | declaration)* EOF;
declaration: functionDeclaration;

//Functions
// Function Declaration
functionDeclaration:
	FN ID namedTypeTuple returnTypeSpecifier THEN functionBody
	// Error cases:
	| FN ID namedTypeTuple THEN functionBody {NotifyErrorListeners(TokenStream.LT(-1), $"Function declaration is missing a return type annotation.", null);
		};
returnTypeSpecifier: voidSpecifier | typeSpecifier;
typeSpecifier:
	COLON (numberType | ID)
	// Error cases:
	| COLON {NotifyErrorListeners(TokenStream.LT(-1), $"A type identifier for \"{TokenStream.LT(-2).Text}\" is missing from this type annotation.", null);
		};
voidSpecifier: COLON VOID;
functionBody: statementGroup;
// Function Call
functionCall: ID orderedTuple;

// Tuples Named Tuples
namedTypeTuple:
	LPAREN namedParameters? RPAREN
	// Error cases:
	| LPAREN namedParameters? {NotifyErrorListeners(TokenStream.LT(-1), "Missing closing \")\" in parenthetical.", null);
		};
namedParameters: namedParameter (COMMA namedParameter)*;
namedParameter:
	ID typeSpecifier
	// Error cases:
	| ID {NotifyErrorListeners(TokenStream.LT(-1), $"Parameter \"{TokenStream.LT(-1).Text}\" is missing a type annotation.", null);
         		};
// Ordered Tuples
orderedTuple:
	LPAREN orderedParameters? RPAREN
	// Error cases:
	| LPAREN orderedParameters? {NotifyErrorListeners(TokenStream.LT(-1), "Missing closing \")\" in parenthetical.", null);
		};
orderedParameters: orderedParameter (COMMA orderedParameter)*;
orderedParameter: ID | literal;

// Grouping
statementGroup: LCURL definiteStatement* RCURL;

// Type Literals
numberType: UNSIGNED? keyword = (INT_KEYWORD | FLOAT_KEYWORD);

// Statements and Expressions
statementKeyword: RETURN | OUT;
definiteStatement:
	statementKeyword? expression SEMICOLON
	// Error cases:
	| statementKeyword? expression { NotifyErrorListeners(TokenStream.LT(-1), "Missing \";\" at end of statement.", null);
		}
	| statementKeyword { NotifyErrorListeners(TokenStream.LT(-1), "Missing expression after keyword in statement.", null);
		};

expression:
	expression op = (STAR | FSLASH) expression
	| expression op = (PLUS | MINUS) expression
	| literal
	| functionCall
	| ID;

// Operators
binaryOperator: STAR | PLUS | MINUS | FSLASH;

// Literal
literal: numericLiteral | stringLiteral;
numericLiteral: NUMBER FLAG?;
stringLiteral: ((DQUOTE .*? DQUOTE) | (SQUOTE .*? SQUOTE)) FLAG?;
