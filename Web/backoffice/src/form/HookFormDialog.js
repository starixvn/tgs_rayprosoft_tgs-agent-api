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
  defaultValues,
  onSubmit,
  onClickSubmit,
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
  addonButtons,
  actionButtonsProps = {},
  formMethods,
  ...props
}) => {
  let methods = useForm({ defaultValues })
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
            <form
              onSubmit={formMethods.handleSubmit(onSubmit)}
              css={css`
                height: 100%;
              `}
            >
              {children}
            </form>
          </FormProvider>
        )}
        showActionButtons={showActionButtons}
        actionButtonsProps={{
          isLoading: isLoading,
          onClickSubmit,
          onClickCancel: formMethods.reset,
          cancelText,
          submitText,
          addonButtons:
            typeof addonButtons === "function"
              ? addonButtons(formMethods)
              : addonButtons,
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
