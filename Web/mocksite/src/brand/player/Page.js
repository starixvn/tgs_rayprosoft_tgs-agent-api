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
        <ViewInfoDialog
          brandCode={brandCode}
          currency={currency}
          username={username}
          dialog={viewInfoDialog}
        />
      )}
      {editInfoDialog.visible && (
        <EditInfoDialog
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

const ViewInfoDialog = ({ brandCode, currency, username, dialog }) => {
  let { data = {} } = usePlayerInfo({ brandCode, currency, username })
  return (
    <Dialog.Modal title={username} showCloseButton {...dialog}>
      <Stack>
        <FieldStack orientation="horizontal">
          <FieldWrapper label="Username">
            <Text.Block>{data?.username}</Text.Block>
          </FieldWrapper>
          <FieldWrapper label="Realname">
            <Text.Block>{data?.realname}</Text.Block>
          </FieldWrapper>
        </FieldStack>

        <FieldWrapper label="Phone">
          <Text.Block>{data?.phone}</Text.Block>
        </FieldWrapper>
        <FieldWrapper label="Email">
          <Text.Block>{data?.email}</Text.Block>
        </FieldWrapper>
        <FieldWrapper label="qq">
          <Text.Block>{data?.qq}</Text.Block>
        </FieldWrapper>
        <FieldWrapper label="wechat">
          <Text.Block>{data?.wechat}</Text.Block>
        </FieldWrapper>
      </Stack>
    </Dialog.Modal>
  )
}

const EditInfoDialog = ({ brandCode, currency, username, dialog }) => {
  let { data = {} } = usePlayerInfo({ brandCode, currency, username })
  let { register, handleSubmit, watch, setError, errors } = useForm()
  let [formData, setSubmit] = useState()
  let { result, isLoading, ...rest } = useUpdatePlayerInfo(
    { brandCode, currency, username },
    formData
  )

  const onSubmit = handleSubmit(setSubmit)

  const reset = () => {
    setSubmit(undefined)
  }

  useEffect(() => {
    if (result) {
      reset()
      dialog.hide()
    }
  }, [result])

  return (
    <Dialog.Modal
      title={username}
      showCloseButton
      showActionButtons
      actionButtonsProps={{
        isLoading,
        onClickSubmit: onSubmit
      }}
      {...dialog}
    >
      <form onSubmit={onSubmit}>
        <FieldStack>
          <FieldStack orientation="horizontal">
            <InputField
              name="username"
              inputRef={register}
              defaultValue={data?.username}
              label="Username"
            />
            <InputField
              name="realname"
              inputRef={register}
              defaultValue={data?.realname}
              label="Realname"
            />
          </FieldStack>
          <InputField
            name="phone"
            inputRef={register}
            defaultValue={data?.phone}
            label="Phone"
          />
          <InputField
            name="email"
            type="email"
            inputRef={register}
            defaultValue={data?.email}
            label="Email"
          />
          <InputField
            name="qq"
            inputRef={register}
            defaultValue={data?.qq}
            label="QQ"
          />
          <InputField
            name="Wechat"
            inputRef={register}
            defaultValue={data?.wechat}
            label="Wechat"
          />
        </FieldStack>
      </form>
    </Dialog.Modal>
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
