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
import { Switch, Route, Link, useParams, useRouteMatch } from "react-router-dom"
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

  let [gameProviderCode, setGameProviderCode] = useState()

  return (
    <Stack padding="major-1" spacing="major-1">
      <Box>
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
      </Box>
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
