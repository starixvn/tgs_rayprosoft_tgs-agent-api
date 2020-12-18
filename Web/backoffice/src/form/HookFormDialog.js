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
  ...props
}) => {
  const methods = useForm()

  useEffect(() => {
    if (result && !error) {
      onSubmitted?.()
      dialog.hide()
      methods.reset()
    }
  }, [result])

  return (
    <>
      <Dialog.Modal
        wrap={(children) => (
          <FormProvider {...methods}>
            <form onSubmit={methods.handleSubmit(onSubmit)}>{children}</form>
          </FormProvider>
        )}
        showActionButtons={showActionButtons}
        actionButtonsProps={{
          isLoading: isLoading,
          onClickCancel: methods.reset,
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
