import React, { useState, useEffect } from "react"
import { css } from "@emotion/react"
import {
  Table,
  Set,
  Button,
  Modal,
  Dialog,
  Card,
  Box,
  Input,
  Group,
  Icon,
  Stack,
  FieldStack,
  InputField
} from "bumbag"
import { faPencilAlt } from "@fortawesome/free-solid-svg-icons"
import { useCaptcha, usePlayerToken } from "../client"
import useSWR from "swr"
import { useRouteMatch, useHistory } from "react-router-dom"

export const LoginDialogButton = ({
  brandCode,
  currency,
  brandToken,
  username,
  ...props
}) => {
  const modal = Modal.useState()
  let [auto, setAuto] = useState()
  return (
    <>
      <Group>
        <Modal.Disclosure
          use={Button}
          onClick={(event) => {
            setAuto(true)
          }}
          {...modal}
        >
          Login
        </Modal.Disclosure>
        <Modal.Disclosure
          use={Button}
          onClick={(event) => {
            setAuto(false)
          }}
          {...modal}
        >
          <Icon type="font-awesome" icon={faPencilAlt} />
        </Modal.Disclosure>
      </Group>
      {modal.visible && (
        <LoginDialog
          brandCode={brandCode}
          currency={currency}
          brandToken={brandToken}
          username={username}
          auto={auto}
          dialog={modal}
        />
      )}
    </>
  )
}

const DefaultPassword = "TgsP@ssw0rd"
const LoginDialog = ({
  brandCode,
  currency,
  brandToken,
  username,
  auto,
  dialog,
  ...props
}) => {
  let match = useRouteMatch()

  let history = useHistory()

  let [fromData, setSubmit] = useState(
    auto ? undefined : { bypassCaptcha: false }
  )
  let [answer, setCaptchaAnswer] = useState("")
  let [password, setPassword] = useState(DefaultPassword)

  let {
    image,
    answer: captcha,
    isLoading: isLoadingCaptcha,
    mutate: mutateCaptcha
  } = useCaptcha(
    dialog.visible ? { brandCode, currency, brandToken } : undefined
  )
  let { token, error, isLoading, mutate } = usePlayerToken(
    dialog.visible ? { brandCode, currency, username } : undefined,
    fromData
  )

  useEffect(() => {
    if (token) {
      let url = `${match.url}/${username}`
      history.push(url)
      dialog.hide()
    }
  }, [token])

  const CaptchaAnsError = 31
  useEffect(() => {
    if (error) {
      setSubmit(undefined)
      mutate(undefined, false)

      if (error.err === CaptchaAnsError) {
        setCaptchaAnswer("")
      }
    }
    console.log("TODO: handle", error)
  }, [error])

  return (
    <Dialog.Modal
      showActionButtons
      title="Login"
      showCloseButton
      actionButtonsProps={{
        isLoading,
        onClickSubmit: () =>
          setSubmit({
            username,
            password,
            captcha,
            answer
          })
      }}
      {...dialog}
    >
      <Stack>
        <FieldStack>
          <InputField label="Username" value={username} readOnly={true} />
          <InputField
            label="Password"
            value={password}
            onChange={(event) => setPassword(event.target.value)}
          />
          <InputField
            label="Captcha"
            addonBefore={
              <Button
                onClick={() => mutateCaptcha(undefined)}
                isLoading={isLoadingCaptcha}
              >
                {image ? (
                  <img src={`data:image/png;base64, ${image}`} />
                ) : (
                  "..."
                )}
              </Button>
            }
            value={answer}
            onChange={(event) => {
              setCaptchaAnswer(event.target.value)
            }}
          />
        </FieldStack>
      </Stack>
    </Dialog.Modal>
  )
}
