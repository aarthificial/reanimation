const fs = require('fs');
const path = require('path');

module.exports = {
    tagFormat: 'v${version}',
    branches: [
        'master',
        {
            name: 'preview',
            prerelease: 'preview'
        }
    ],
    plugins: [
        [
            '@semantic-release/commit-analyzer',
            {
                preset: 'angular'
            }
        ],
        [
            '@semantic-release/release-notes-generator',
            {
                writerOpts: {
                    headerPartial: fs.readFileSync(path.resolve(__dirname, './.github/templates/header.hbs'), 'utf-8')
                }
            }
        ],
        [
            '@semantic-release/changelog',
            {
                preset: 'angular'
            }
        ],
        [
            '@semantic-release/npm',
            {
                npmPublish: false
            }
        ],
        [
            '@semantic-release/git',
            {
                assets: [
                    'package.json',
                    'CHANGELOG.md'
                ],
                message: 'chore(release): ${nextRelease.version} [skip ci]\n\n${nextRelease.notes}'
            }
        ],
        '@semantic-release/github'
    ]
}