lexer grammar RadLexer;

channels {
  WHITESPACE_CHANNEL,
  COMMENTS_CHANNEL
}

fragment LOWERCASE  : [a-z] ;
fragment UPPERCASE  : [A-Z] ;

// Keywords
FN: 'fn';

// Type Keywords
VOID: 'void';
UNSIGNED: 'unsigned';
INT_KEYWORD: 'int' | 'byte' | 'i16' | 'i32' | 'i64' | 'i128';
FLOAT_KEYWORD: 'float' | 'f16' | 'f32' | 'f64' | 'f80' | 'f128';
BOOL_KEYWORD: 'bool';

// Statement keywords
RETURN: 'return';
OUT: 'out';

// Identifier
ID: (FLAG | LOWERCASE | UPPERCASE)+(LOWERCASE | UPPERCASE | NUMBER | '-')*;
FLAG: LOWERCASE+;

// Groupings and associations
LPAREN: '(';
RPAREN: ')';
LCURL: '{';
RCURL: '}';
DQUOTE: '"';
SQUOTE: '\'';
COMMA: ',';
SEMICOLON: ';';
COLON: ':';
THEN: '=>';

// Binary Operators
STAR: '*';
PLUS: '+';
MINUS: '-';
FSLASH: '/';

// Literals
NUMBER: [0-9]+;

// Whitespace
WHITESPACE: (' '|'\t')+ -> channel(WHITESPACE_CHANNEL);
NEWLINE: ('\r'? '\n' | '\r')+ -> channel(WHITESPACE_CHANNEL);
