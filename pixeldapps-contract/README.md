# pixeldapps-contract

Here lies the smart contract code for the following three games:

1. Pixel Pets
2. Crypto Heroes
3. Chain Team Tactics

As well as "presale" info for Pixel Pets eggs, NEP info for game asstes and $PXT, and helper contracts to support the games.



# Installation Steps
- To install it \
`npm install`

- To Build it \
`npm run build`

- Create testnet account \
`near create-account <your-account-id.testnet> --useFaucet`

- To Deploy the contract \
`near deploy <created-account> build/release/<build_file_name>.wasm`