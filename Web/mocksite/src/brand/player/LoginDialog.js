import React, { useState, useEffect } from "react"
import { css } from "@emotion/react"
import {
  Table,
  Set,
  Button,
  Dialog,
  Modal,
  Card,
  Box,
  Input,
  Group
} from "bumbag"
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
  return (
    <>
      <Group>
        <Modal.Disclosure use={Button} {...modal}>
          Login
        </Modal.Disclosure>
      </Group>
      {modal.visible && (
        <LoginDialog
          brandCode={brandCode}
          currency={currency}
          brandToken={brandToken}
          username={username}
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
  dialog,
  ...props
}) => {
  let match = useRouteMatch()

  let history = useHistory()

  let [fromData, setSubmit] = useState()
  let [answer, setCaptchaAnswer] = useState("")
  let [password, setPassword] = useState(DefaultPassword)

  let { image, answer: captcha } = useCaptcha(
    dialog.visible ? { brandCode, currency, brandToken } : undefined
  )
  let { token, error } = usePlayerToken(
    dialog.visible ? { brandCode, currency, username } : undefined,
    fromData
  )

  useEffect(() => {
    if (dialog.visible && token) {
      let url = `${match.url}/${username}`
      history.push(url)
      dialog.hide()
    }
  }, [dialog.visible, token])

  return (
    <Modal {...dialog}>
      <Card>
        <Group>
          <Button isStatic>
            {image ? <img src={`data:image/png;base64, ${image}`} /> : null}
          </Button>
          <Input
            value={answer}
            onChange={(event) => {
              setCaptchaAnswer(event.target.value)
            }}
          />
        </Group>
        <Box>
          <Group>
            <Button isStatic>Password</Button>
            <Input
              value={password}
              onChange={(event) => setPassword(event.target.value)}
            />
          </Group>
        </Box>
        <Button
          onClick={(event) => {
            setSubmit({
              username,
              password,
              captcha,
              answer
            })
          }}
        >
          Submit
        </Button>
        <Modal.Disclosure use={Button} {...dialog}>
          Close
        </Modal.Disclosure>
      </Card>
    </Modal>
  )
}
