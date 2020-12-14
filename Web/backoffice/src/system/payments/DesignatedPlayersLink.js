import React, { useState, useEffect, useMemo } from "react"
import { css, Global } from "@emotion/react"
import { Formik, Form, Field } from "formik"
import {
  Heading,
  Stack,
  Button,
  Set,
  InputField,
  Text,
  Box,
  Input,
  Link,
  Dialog,
  Card,
  List
} from "bumbag"
import LoadingBackdrop from "../../dialog/LoadingBackdrop"
import Alert from "../../dialog/Alert"
import { usePaymentDesignatedUsers } from "./client"
import Disclosure from "../../dialog/Disclosure"
import { CloseButtonFooter, useModalState } from "../../dialog/Modal"
import { Trans, useTrans } from "../../i18n"
import { CloseText } from "../../button/ActionButtons"

export const DesignatedPlayersLink = ({ record = {}, ...props }) => {
  let dialog = useModalState()

  let { id, designatedUsers } = record
  if (!designatedUsers) {
    return <Trans zh="无" en="None" />
  }

  return (
    <>
      <Disclosure use={Link} {...dialog}>
        <Trans zh="有" en="Users" />
      </Disclosure>
      {dialog.visible && <PlayersDialog id={id} dialog={dialog} />}
    </>
  )
}

const PlayersDialog = ({ id, dialog }) => {
  let title = useTrans({
    zh: "特定用户",
    en: "Designated Users"
  })
  let { data, isLoading, error, mutate } = usePaymentDesignatedUsers(
    dialog.visible && id
  )
  if (isLoading) {
    return <LoadingBackdrop />
  }

  if (error) {
    return <Alert {...error} onClickClose={dialog.hide} />
  }

  let players = data?.players ?? []
  return (
    <Dialog.Modal
      title={title}
      footer={
        <Box alignX="center" width="100%">
          <Button onClick={dialog.hide}>
            <CloseText />
          </Button>
        </Box>
      }
      {...dialog}
    >
      <Card variant="bordered" padding="1em">
        <List>
          {players.map((player) => (
            <List.Item>{player}</List.Item>
          ))}
        </List>
      </Card>
    </Dialog.Modal>
  )
}
