import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/core"
import { Formik, Form } from "formik"
import { Dialog as _Dialog } from "bumbag"
import { CancelText, SubmitText } from "../button/ActionButtons"
import AlertDialog from "../dialog/Alert"

export const Dialog = ({
  initialValues = {},
  onSubmit,
  isLoading,
  result,
  error,
  onCloseErrorAlert,
  onSubmitted,
  title,
  cancelText = <CancelText />,
  submitText = <SubmitText />,
  children,
  dialog,
  hideOnClickOutside,
  finalFocusRef,
  ...props
}) => {
  return (
    <Formik initialValues={initialValues} onSubmit={onSubmit}>
      {({ submitForm, resetForm }) => {
        useEffect(() => {
          if (result && !error) {
            onSubmitted && onSubmitted()
            dialog.hide()
            resetForm()
          }
        }, [result])

        return (
          <>
            <_Dialog.Modal
              title={title}
              showActionButtons
              actionButtonsProps={{
                isLoading: isLoading,
                onClickSubmit: submitForm,
                onClickCancel: resetForm,
                cancelText,
                submitText
              }}
              hideOnClickOutside={false}
              unstable_finalFocusRef={finalFocusRef}
              {...dialog}
            >
              <Form>{children}</Form>
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
