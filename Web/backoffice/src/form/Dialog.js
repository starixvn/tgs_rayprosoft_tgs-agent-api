import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/core"
import { Formik, Form } from "formik"
import { Dialog as _Dialog } from "bumbag"
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
          <>
            <_Dialog.Modal
              title={title}
              footer={footer}
              showActionButtons={showActionButtons}
              actionButtonsProps={{
                isLoading: isLoading,
                onClickSubmit: clickSubmit
                  ? clickSubmit(formProps)
                  : submitForm,
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
              css={css`
                & .bb-DialogContent {
                  position: relative;
                }
              `}
              {...dialog}
              {...props}
            >
              <Form>
                {isChildrenFunction && children(formProps)}
                {!isChildrenFunction && children}
              </Form>
            </_Dialog.Modal>
            {error && (
              <AlertDialog {...error} onClickClose={onCloseErrorAlert} />
            )}
          </>
        )
      }}
    </Formik>
  )
}

export default Dialog
