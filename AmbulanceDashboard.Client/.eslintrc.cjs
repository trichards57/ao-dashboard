module.exports = {
    root: true,
    env: { browser: true, es2020: true },
    extends: [
        'eslint:recommended',
        'plugin:@typescript-eslint/recommended',
        'plugin:react-hooks/recommended',
        'airbnb',
        'airbnb/hooks',
        'airbnb-typescript',
        'plugin:react/jsx-runtime',
        'plugin:import/recommended',
        'plugin:import/typescript'
    ],
    ignorePatterns: ['dist', '.eslintrc.cjs', 'vite.config.ts'],
    parser: '@typescript-eslint/parser',
    plugins: ['react-refresh', 'import'],
    rules: {
        'react-refresh/only-export-components': [
            'warn',
            { allowConstantExport: true },
        ],
        "linebreak-style": ['error', 'windows'],
        "@typescript-eslint/quotes": ['error', 'double'],
        "import/order": [
            "error",
            {
                "groups": ["builtin", "external", "parent", "sibling", "index"],
                "pathGroups": [
                    {
                        "pattern": "@custom-lib/**",
                        "group": "external",
                        "position": "after"
                    }
                ],
                "pathGroupsExcludedImportTypes": ["builtin"],
                "alphabetize": {
                    "order": "asc"
                },
                "newlines-between": "always"
            }
        ],
        "sort-imports": [
            "error",
            {
                "allowSeparatedGroups": true,
                "ignoreDeclarationSort": true,
            }
        ],
        "no-duplicate-imports": "error",
        "no-multiple-empty-lines": [
            "error",
            {
                "max": 1,
                "maxEOF": 0,
                "maxBOF": 0
            }
        ],
        "import/first": "error",
        "import/newline-after-import": "error",
        "import/no-duplicates": "error",
        "import/no-unassigned-import": "error"
    },
    parserOptions: {
        project: './tsconfig.json',
        tsconfigRootDir: __dirname
    }
}
