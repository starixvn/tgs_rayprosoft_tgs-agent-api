import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/core"
import client from "../../client"
import { Alert, Button, Text, Modal } from "bumbag"
import { Delete as DeleteMenu } from "./menu"
import { useIsDisplayMenuByRoute } from "../../user/context"
import AsyncFormSubmitModal from "../../dialog/AsyncFormSubmitModal"

export const DeleteButton = ({ id, name, onSubmitted, ...props }) => {
  let dialog = Modal.useState()
  let disclosureRef = useRef()

  let isDisplay = useIsDisplayMenuByRoute([DeleteMenu])
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
      <SubmitDialog
        id={id}
        name={name}
        onSubmitted={onSubmitted}
        dialog={dialog}
        finalFocusRef={disclosureRef}
      />
    </>
  )
}

const SubmitDialog = ({
  id,
  name = "",
  dialog,
  onSubmitted,
  finalFocusRef,
  ...props
}) => {
  const deferFn = ([_data]) => {
    return client.delete(`/api/role/${id}`)
  }

  return (
    <AsyncFormSubmitModal
      deferFn={deferFn}
      title={title(name)}
      dialog={dialog}
      onSubmitted={onSubmitted}
      finalFocusRef={finalFocusRef}
    >
      <Alert title={warningTitle(name)} type="warning" variant="">
        <Text color="danger">{warningContent}</Text>
      </Alert>
    </AsyncFormSubmitModal>
  )
}

//TODO: i18n
const deleteLabel = () => "删除"

const title = () => "删除权限组"

const warningTitle = (name = "") => `确认删除 “${name}” 权限组？`

const warningContent = () =>
  "如果仍有管理员正使用此权限组，该权限组将不能够被删除"
