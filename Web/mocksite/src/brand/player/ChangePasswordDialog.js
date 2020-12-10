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

export const ChangePasswordDialog = ({
  brandCode,
  currency,
  username,
  dialog
}) => {
  let { register, handleSubmit, watch, setError, errors } = useForm()
  let [formData, setSubmit] = useState()
  let { result, error, isLoading, mutate, ...rest } = useChangePlayerPassword(
    { brandCode, currency, username },
    formData
  )

  const onSubmit = handleSubmit(setSubmit)

  const reset = () => {
    setSubmit(undefined)
    mutate(undefined, false)
  }

  useEffect(() => {
    if (result) {
      reset()
      dialog.hide()
    }
  }, [result])

  const errorDialog = Modal.useState()
  useEffect(() => {
    if (error) {
      errorDialog.show()
    }
  }, [error])

  return (
    <>
      <Dialog.Modal
        title={username}
        showCloseButton
        showActionButtons
        hideOnClickOutside={false}
        actionButtonsProps={{
          isLoading,
          onClickSubmit: onSubmit
        }}
        {...dialog}
      >
        <FieldStack>
          <InputField name="oldpwd" inputRef={register} label="Old Password" />
          <InputField name="newpwd" inputRef={register} label="New Password" />
          <InputField
            name="confirmpwd"
            inputRef={register}
            label="Confirm Password"
          />
        </FieldStack>
      </Dialog.Modal>
      {error && (
        <Modal.Backdrop zIndex="19900410" {...errorDialog}>
          <Dialog.Modal
            title="Error"
            showCloseButton
            type="warning"
            hideBackdrop={true}
            onClickClose={() => {
              reset()
            }}
            {...errorDialog}
          >
            <Text.Block>{JSON.stringify(error)}</Text.Block>
          </Dialog.Modal>
        </Modal.Backdrop>
      )}
    </>
  )
}
