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
import { useCaptcha, usePlayerToken } from "../../client/brand/player"
import { useRouteMatch, useHistory } from "react-router-dom"
import { useForm } from "react-hook-form"

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

  let { register, handleSubmit, watch, setError, errors } = useForm()
  let [fromData, setSubmit] = useState(
    auto ? undefined : { bypassCaptcha: false }
  )

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
  const onSubmit = handleSubmit(setSubmit)

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
      setSubmit(auto ? undefined : { bypassCaptcha: false })
      mutate(undefined, false)

      if (error.err === CaptchaAnsError) {
        setError("answer", {
          type: "answer",
          shouldFocus: true
        })
      }
    }
  }, [error])

  return (
    <Dialog.Modal
      showActionButtons
      title="Login"
      showCloseButton
      actionButtonsProps={{
        isLoading,
        onClickSubmit: onSubmit
      }}
      {...dialog}
    >
      <form onSubmit={onSubmit}>
        <Stack>
          <FieldStack>
            <InputField
              name="username"
              inputRef={register}
              label="Username"
              defaultValue={username}
              readOnly={true}
            />
            <InputField
              name="password"
              inputRef={register({ required: true })}
              label="Password"
              defaultValue={DefaultPassword}
            />
            <InputField
              name="answer"
              inputRef={register({ required: true })}
              state={errors.answer ? "danger" : ""}
              label="Captcha"
              addonBefore={
                <Button
                  onClick={() => mutateCaptcha(undefined)}
                  isLoading={isLoadingCaptcha}
                >
                  {image ? (
                    <>
                      <img src={`data:image/png;base64, ${image}`} />
                      <input
                        name="captcha"
                        ref={register}
                        value={captcha}
                        readOnly
                        css={css`
                          display: none;
                        `}
                      />
                    </>
                  ) : (
                    "..."
                  )}
                </Button>
              }
            />
          </FieldStack>
        </Stack>
      </form>
    </Dialog.Modal>
  )
}
