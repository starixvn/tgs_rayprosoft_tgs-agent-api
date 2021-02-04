import React, { useState, useEffect } from "react"
import { css } from "@emotion/react"
import {
  Table,
  Set,
  Button,
  Box,
  Stack,
  PageWithHeader,
  TopNav,
  Popover,
  Input,
  InputField,
  FieldStack,
  Checkbox,
  CheckboxField,
  Text,
  FieldWrapper,
  Dialog,
  Modal,
  Group
} from "bumbag"
import {
  Switch,
  Route,
  Link,
  useHistory,
  useParams,
  useRouteMatch
} from "react-router-dom"
import { useForm } from "react-hook-form"
import {
  useCaptcha,
  useCheckPlayerUsername,
  useCreatePlayer
} from "../client/brand/player"

export const CreatePlayerDialog = ({
  brandCode,
  currency,
  dialog,
  onPlayerCreated
}) => {
  /*
  let {
    image,
    answer: captcha,
    isLoading: isLoadingCaptcha,
    mutate: mutateCaptcha
  } = useCaptcha({ brandCode, currency })
  */

  let {
    register,
    handleSubmit,
    watch,
    setError,
    errors,
    clearErrors
  } = useForm()
  let isTestPlayer = watch("testplayer")
  let isAgent = watch("agent")
  let [formData, setSubmit] = useState()
  let { result, error, isLoading } = useCreatePlayer(
    { brandCode, currency },
    formData
  )
  const onSubmit = handleSubmit((data) => {
    setSubmit({
      ...data
    })
  })

  useEffect(() => {
    if (result) {
      dialog.hide()
      onPlayerCreated?.()
    }
  }, [result])

  const errorDialog = Modal.useState()
  useEffect(() => {
    if (error) {
      errorDialog.show()
    }
  }, [error])

  let [checkUsernameFormData, setCheckUsernameFormData] = useState()
  let { result: usernameResult, mutate } = useCheckPlayerUsername(
    { brandCode, currency },
    checkUsernameFormData
  )

  useEffect(() => {
    if (usernameResult?.isexist) {
      setError("username", {
        type: "exist",
        message: "Username exists"
      })
    } else if (!usernameResult?.isexist) {
      clearErrors("username")
    }
  }, [usernameResult])

  return (
    <Dialog.Modal
      title={brandCode}
      showCloseButton
      hideOnClickOutside={false}
      showActionButtons
      actionButtonsProps={{
        isLoading,
        onClickSubmit: onSubmit
      }}
      {...dialog}
    >
      <FieldStack>
        <InputField
          name="username"
          onChange={(event) => {
            let username = event.target.value
            if (username) {
              setCheckUsernameFormData(username)
            } else {
              mutate(undefined, false)
              clearErrors("")
            }
          }}
          state={errors.username ? "danger" : ""}
          validationText={errors?.username?.message}
          inputRef={register}
          label="Username"
        />
        <FieldStack orientation="horizontal">
          <CheckboxField
            name="testplayer"
            defaultChecked={true}
            checkboxRef={register}
            label="Test Player"
            checkboxLabel={isTestPlayer ? "Yes" : "No"}
          />
          <CheckboxField
            name="agent"
            checkboxRef={register}
            label="Agent"
            checkboxLabel={isAgent ? "Yes" : "No"}
          />
        </FieldStack>
        <InputField name="pwd" inputRef={register} label="Password" />
        <InputField
          name="confirmpwd"
          inputRef={register}
          label="Confirm Password"
        />
        <FieldStack orientation="horizontal">
          <InputField name="currency" inputRef={register} label="Currency" />
          <InputField name="lang" inputRef={register} label="lang" />
        </FieldStack>
        <InputField
          name="invitationcode"
          inputRef={register}
          label="Invitation code"
        />
        {/* <InputField
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
        /> */}
        {error && (
          <Modal.Backdrop zIndex="19900410" {...errorDialog}>
            <Dialog.Modal
              title="Error"
              showCloseButton
              type="warning"
              hideBackdrop={true}
              {...errorDialog}
            >
              <Text.Block>{JSON.stringify(error)}</Text.Block>
            </Dialog.Modal>
          </Modal.Backdrop>
        )}
      </FieldStack>
    </Dialog.Modal>
  )
}
