import React, { useState, useEffect } from "react"
import { css } from "@emotion/react"
import {
  Table,
  Set,
  Button,
  Stack,
  Box,
  PageWithHeader,
  TopNav,
  DropdownMenu,
  Group,
  Card
} from "bumbag"
import {
  Switch,
  Route,
  Link,
  useParams,
  useRouteMatch,
  useLocation,
  useHistory
} from "react-router-dom"
import {
  State,
  useCurrency,
  useLang,
  usePlatform,
  useSetCurrency,
  useSetLang,
  useSetPlatform
} from "../State"
import BrandHeader from "../Header"
import PlatformDropdownMenu from "../../platform/DropdownMenu"
import LangDropdownMenu from "../../language/DropdownMenu"
import { GameProviderDropdownMenu } from "../../game/provider/DropdownMenu"
import { GameList } from "../../game/List"
import { useLogout, usePlayerToken } from "../client"

export const PlayerPage = ({ brandToken, ...props }) => {
  return (
    <PageWithHeader header={<Header />} {...props}>
      <Body brandToken={brandToken} />
    </PageWithHeader>
  )
}

const Body = ({ brandToken }) => {
  let { brandCode, username } = useParams()
  let currency = useCurrency()
  let platform = usePlatform()
  let language = useLang()

  let { token: playerToken } = usePlayerToken({ brandCode, currency, username })
  let [gameProviderCode, setGameProviderCode] = useState()

  let [token, setLogoutToken] = useState()
  let { result, error, isLoading, ...rest } = useLogout(
    { brandCode, currency, username },
    token
  )
  let history = useHistory()
  useEffect(() => {
    if (result) {
      history.push(`/${brandCode}/${currency}`)
    }
  })

  return (
    <Stack padding="major-1" spacing="major-1">
      <Set>
        <DropdownMenu
          menu={
            <PlayerMenu
              logout={() => setLogoutToken(playerToken)}
              isLoading={isLoading}
            />
          }
        >
          <Button iconAfter="chevron-down" isLoading={isLoading}>
            {username}
          </Button>
        </DropdownMenu>
        <GameProviderDropdownMenu
          brandCode={brandCode}
          currency={currency}
          brandToken={brandToken}
          username={username}
          platform={platform}
          language={language}
          value={gameProviderCode}
          onChange={setGameProviderCode}
        />
      </Set>
      <Box>
        <GameList
          brandCode={brandCode}
          username={username}
          currency={currency}
          platform={platform}
          language={language}
          gameProviderCode={gameProviderCode}
        />
      </Box>
    </Stack>
  )
}

const PlayerMenu = ({ logout }) => {
  return (
    <>
      <DropdownMenu.Item onClick={logout}>Logout</DropdownMenu.Item>
    </>
  )
}

const Header = () => {
  let platform = usePlatform()
  let setPlatform = useSetPlatform()

  let lang = useLang()
  let setLang = useSetLang()
  return (
    <BrandHeader
      navItems={
        <>
          <TopNav.Item>
            <PlatformDropdownMenu value={platform} onChange={setPlatform} />
          </TopNav.Item>
          <TopNav.Item>
            <LangDropdownMenu value={lang} onChange={setLang} />
          </TopNav.Item>
        </>
      }
    />
  )
}

export default PlayerPage
