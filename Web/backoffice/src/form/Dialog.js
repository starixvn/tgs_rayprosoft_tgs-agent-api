import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/react"
import { Formik, Form } from "formik"
import { Dialog as _Dialog, Button, Box } from "bumbag"
import { CancelText, SubmitText } from "../button/ActionButtons"
import AlertDialog from "../dialog/Alert"

export const Dialog = ({
  initialValues = {},
  onReset,
  onSubmit,
  isLoading,
  result,
  error,
  onCloseErrorAlert,
  onSubmitted,
  title,
  footer,
  showActionButtons = true,
  cancelText = <CancelText />,
  submitText = <SubmitText />,
  clickSubmit,
  addonButtons,
  children,
  dialog,
  hideOnClickOutside,
  finalFocusRef,
  ...props
}) => {
  return (
    <Formik initialValues={initialValues} onSubmit={onSubmit}>
      {(formProps) => {
        let { submitForm, resetForm } = formProps

        const reset = () => {
          resetForm()
          onReset && onReset()
        }

        useEffect(() => {
          if (result && !error) {
            onSubmitted && onSubmitted()
            dialog.hide()
            reset()
          }
        }, [result])

        let isChildrenFunction = typeof children === "function"
        return (
          <Form>
            <_Dialog.Modal
              wrap={(children) => {
                return (
                  <Form
                    css={css`
                      display: flex;
                      flex-direction: column;
                      height: 100%;
                    `}
                  >
                    {children}
                  </Form>
                )
              }}
              title={title}
              footer={footer}
              showActionButtons={showActionButtons}
              actionButtonsProps={{
                isLoading: isLoading,
                onClickSubmit: clickSubmit?.(formProps),
                onClickCancel: reset,
                cancelText,
                submitText,
                addonButtons:
                  typeof addonButtons === "function"
                    ? addonButtons(formProps)
                    : addonButtons
              }}
              hideOnClickOutside={false}
              unstable_finalFocusRef={finalFocusRef}
              {...dialog}
              {...props}
            >
              {isChildrenFunction && children(formProps)}
              {!isChildrenFunction && children}
            </_Dialog.Modal>
            {error && (
              <AlertDialog {...error} onClickClose={onCloseErrorAlert} />
            )}
          </Form>
        )
      }}
    </Formik>
  )
}

export const Footer = Dialog.Footer

export const ConfirmButtonFooter = ({
  isLoading,
  disabled,
  width = "100%",
  alignX = "center",
  ...props
}) => {
  return (
    <Box width={width} alignX={alignX} {...props}>
      <Button
        palette="primary"
        type="submit"
        isLoading={isLoading}
        disabled={disabled}
      >
        <SubmitText />
      </Button>
    </Box>
  )
}

export default Dialog
