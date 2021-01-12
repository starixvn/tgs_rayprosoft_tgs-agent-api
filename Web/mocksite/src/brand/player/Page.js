import React, { useState, useEffect } from "react"
import { css } from "@emotion/react"
import {
  Table,
  Set,
  Button,
  Stack,
  FieldStack,
  Box,
  PageWithHeader,
  TopNav,
  DropdownMenu,
  Group,
  Card,
  Dialog,
  Modal,
  Text,
  FieldWrapper,
  InputField
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
import {
  useChangePlayerPassword,
  useLogout,
  usePlayerInfo,
  usePlayerToken,
  useUpdatePlayerInfo
} from "../../client/brand/player"
import { useForm } from "react-hook-form"
import { ChangePasswordDialog } from "./ChangePasswordDialog"
import { EditPlayerInfoDialog } from "./InfoDialog"

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
      <Set>
        <PlayerDropdownMenu
          brandCode={brandCode}
          currency={currency}
          username={username}
        />
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

const PlayerDropdownMenu = ({ brandCode, currency, username }) => {
  let { token: playerToken, error: playerTokenError } = usePlayerToken({
    brandCode,
    currency,
    username
  })

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
  }, [result])

  useEffect(() => {
    if (playerTokenError) {
      history.push(`/${brandCode}/${currency}`)
    }
  }, [playerTokenError])

  const viewInfoDialog = Modal.useState()
  const editInfoDialog = Modal.useState()
  const changePasswordDialog = Modal.useState()

  return (
    <>
      <DropdownMenu
        menu={
          <PlayerMenu
            playerInfoDialog={viewInfoDialog}
            editPlayerInfoDialog={editInfoDialog}
            editPasswordDialog={changePasswordDialog}
            logout={() => setLogoutToken(playerToken)}
          />
        }
      >
        <Button iconAfter="chevron-down" isLoading={isLoading}>
          {username}
        </Button>
      </DropdownMenu>
      {viewInfoDialog.visible && (
        <playerInfoDialog
          brandCode={brandCode}
          currency={currency}
          username={username}
          dialog={viewInfoDialog}
        />
      )}
      {editInfoDialog.visible && (
        <EditPlayerInfoDialog
          brandCode={brandCode}
          currency={currency}
          username={username}
          dialog={editInfoDialog}
        />
      )}
      {changePasswordDialog.visible && (
        <ChangePasswordDialog
          brandCode={brandCode}
          currency={currency}
          username={username}
          dialog={changePasswordDialog}
        />
      )}
    </>
  )
}

const PlayerMenu = ({
  playerInfoDialog,
  editPlayerInfoDialog,
  editPasswordDialog,
  logout
}) => {
  return (
    <>
      <DropdownMenu.Item onClick={playerInfoDialog.show}>
        View Info
      </DropdownMenu.Item>
      <DropdownMenu.Item onClick={editPlayerInfoDialog.show}>
        Edit Info
      </DropdownMenu.Item>
      <DropdownMenu.Item onClick={editPasswordDialog.show}>
        Change Password
      </DropdownMenu.Item>
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
