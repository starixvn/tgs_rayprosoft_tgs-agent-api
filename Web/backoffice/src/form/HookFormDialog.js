import React, {
  useState,
  useEffect,
  useMemo,
  useRef,
  createContext,
  useContext
} from "react"
import { css, Global } from "@emotion/react"
import { useForm, FormProvider, useFormContext } from "react-hook-form"
import { Dialog, Button, Box } from "bumbag"
import { CancelText, SubmitText } from "../button/ActionButtons"
import AlertDialog from "../dialog/Alert"

export const HookFormDialog = ({
  onSubmit,
  isLoading,
  result,
  error,
  onCloseErrorAlert,
  onSubmitted,
  children,
  dialog,
  hideOnClickOutside = false,
  showActionButtons = true,
  cancelText = <CancelText />,
  submitText = <SubmitText />,
  actionButtonsProps = {},
  formMethods,
  ...props
}) => {
  let methods = useForm()
  formMethods = formMethods ?? methods

  useEffect(() => {
    if (result && !error) {
      onSubmitted?.()
      dialog.hide()
      formMethods.reset()
    }
  }, [result])

  return (
    <>
      <Dialog.Modal
        wrap={(children) => (
          <FormProvider {...formMethods}>
            <form onSubmit={formMethods.handleSubmit(onSubmit)}>
              {children}
            </form>
          </FormProvider>
        )}
        showActionButtons={showActionButtons}
        actionButtonsProps={{
          isLoading: isLoading,
          onClickCancel: formMethods.reset,
          cancelText,
          submitText,
          ...actionButtonsProps
        }}
        hideOnClickOutside={hideOnClickOutside}
        {...dialog}
        {...props}
      >
        {children}
      </Dialog.Modal>
      {error && <AlertDialog {...error} onClickClose={onCloseErrorAlert} />}
    </>
  )
}

export default HookFormDialog
