import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/core"
import { useLocation, useRouteMatch } from "react-router-dom"
import Async from "react-async"
import client from "../../client"
import { Formik, Form, Field, useFormikContext } from "formik"
import {
  Stack,
  Flex,
  Box,
  Card,
  Set,
  Alert,
  Button,
  Text,
  Label,
  Input,
  InputField,
  Checkbox,
  FieldStack,
  SelectField,
  Dialog,
  Modal,
  Table,
  useClassName
} from "bumbag"
import LoadingBackdrop from "../../dialog/LoadingBackdrop"
import AlertDialog from "../../dialog/Alert"
import { Delete as DeleteMenu } from "./menu"
import { useIsDisplayMenu } from "../../user/context"

export const DeleteButton = ({ id, name, onSubmitted, ...props }) => {
  let dialog = Modal.useState()
  let disclosureRef = useRef()

  let { path = "" } = useRouteMatch()
  let [_, ...codes] = path.split("/")
  let isDisplay = useIsDisplayMenu([...codes, DeleteMenu])
  if (!isDisplay) {
    return null
  }

  return (
    <>
      <Modal.Disclosure
        use={Button}
        ref={disclosureRef}
        css={css`
          flex: unset;
        `}
        palette="danger"
        {...dialog}
      >
        {deleteLabel()}
      </Modal.Disclosure>
      {dialog.visible && (
        <SubmitDialog
          id={id}
          name={name}
          onSubmitted={onSubmitted}
          dialog={dialog}
          finalFocusRef={disclosureRef}
        />
      )}
    </>
  )
}

const SubmitDialog = ({
  id,
  name = "",
  onSubmitted,
  dialog,
  finalFocusRef,
  ...props
}) => {
  const deferFn = ([_data]) => {
    return client.delete(`/api/role/${id}`)
  }

  return (
    <Async deferFn={deferFn}>
      {(asyncDelete) => {
        return (
          <>
            <Dialog.Modal
              title={title(name)}
              showActionButtons
              actionButtonsProps={{
                isLoading: asyncDelete.isPending,
                onClickSubmit: asyncDelete.run
              }}
              hideOnClickOutside={false}
              unstable_finalFocusRef={finalFocusRef}
              {...dialog}
            >
              <Alert title={warningTitle(name)} type="warning" variant="">
                <Text color="danger">{warningContent}</Text>
              </Alert>
            </Dialog.Modal>
            <Async.Rejected>
              {(error) => <AlertDialog {...error} finalFocusRef={inputRef} />}
            </Async.Rejected>
            <Async.Fulfilled>
              {(_) => {
                useEffect(() => {
                  dialog.hide()
                  onSubmitted && onSubmitted()
                })

                return null
              }}
            </Async.Fulfilled>
          </>
        )
      }}
    </Async>
  )
}

//TODO: i18n
const deleteLabel = () => "删除"

const title = () => "删除权限组"

const warningTitle = (name = "") => `确认删除 “${name}” 权限组？`

const warningContent = () =>
  "如果仍有管理员正使用此权限组，该权限组将不能够被删除"
