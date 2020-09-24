import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/core"
import { useLocation, useRouteMatch } from "react-router-dom"
import Async from "react-async"
import { Formik, useFormikContext } from "formik"
import { Dialog as _Dialog } from "bumbag"
import AlertDialog from "./Alert"

export const Dialog = ({
  deferFn,
  title,
  dialog = {},
  initialValues = {},
  onSubmitted,
  finalFocusRef,
  children,
  ...props
}) => {
  return (
    <Async deferFn={deferFn}>
      {(asyncSubmit) => {
        return (
          <>
            <Formik initialValues={initialValues} onSubmit={asyncSubmit.run}>
              <Body title={title} dialog={dialog} finalFocusRef={finalFocusRef}>
                {children}
              </Body>
            </Formik>
            <Async.Rejected>
              {(error) => (
                <AlertDialog {...error} finalFocusRef={finalFocusRef} />
              )}
            </Async.Rejected>
            <Async.Fulfilled>
              {(_) => {
                useEffect(() => {
                  if (dialog.visible) {
                    dialog.hide()
                    onSubmitted && onSubmitted()
                  }
                }, [dialog.visible])

                return null
              }}
            </Async.Fulfilled>
          </>
        )
      }}
    </Async>
  )
}

const Body = ({ title, dialog, finalFocusRef, children, ...props }) => {
  const { submitForm, resetForm, isPending } = useFormikContext() ?? {}
  return (
    <_Dialog.Modal
      title={title}
      showActionButtons
      actionButtonsProps={{
        isLoading: isPending,
        onClickSubmit: submitForm,
        onClickCancel: resetForm
      }}
      hideOnClickOutside={false}
      unstable_finalFocusRef={finalFocusRef}
      {...dialog}
    >
      {children}
    </_Dialog.Modal>
  )
}

export default Dialog
