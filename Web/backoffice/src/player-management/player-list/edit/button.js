import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/core"
import Async, { useAsync, IfRejected, IfFulfilled } from "react-async"
import { useLocation, useRouteMatch } from "react-router-dom"
import client from "../../../client"
import { Formik, Field, Form, useFormikContext } from "formik"
import {
  Spinner,
  Stack,
  Flex,
  Box,
  Card,
  Set,
  Button as _Button,
  Tabs,
  Text,
  Label,
  Input,
  Checkbox,
  FieldStack,
  FieldWrapper,
  SelectField,
  Dialog,
  Modal,
  Table,
  useClassName
} from "bumbag"
import { Update as UpdateMenu } from "../menu"
import { useIsDisplayMenuByRoute } from "../../../user/context"
import { Tab as PlayerTab, Panel as PlayerPanel } from "./player"
import { Tab as ContractTab, Panel as ContractPanel } from "./contact"
import { Tab as BankCardTab, Panel as BankCardPanel } from "./bankCard"
import { Tab as PasswordTab, Panel as PasswordPanel } from "./password"

export const Button = ({ player, onSubmitted, ...props }) => {
  let dialog = Modal.useState()
  let disclosureRef = useRef()

  let isDisplay = useIsDisplayMenuByRoute([UpdateMenu])
  if (!isDisplay) {
    return null
  }

  return (
    <>
      <Modal.Disclosure
        use={_Button}
        ref={disclosureRef}
        css={css`
          flex: unset;
        `}
        {...dialog}
      >
        {edit()}
      </Modal.Disclosure>
      <TabsDialog
        onSubmitted={onSubmitted}
        dialog={dialog}
        finalFocusRef={disclosureRef}
      />
    </>
  )
}

const TabsDialog = ({ dialog, finalFocusRef }) => {
  return (
    <Dialog.Modal
      title={title()}
      unstable_finalFocusRef={finalFocusRef}
      {...dialog}
    >
      <Tabs>
        <Tabs.List>
          <PlayerTab />
          <ContractTab />
          <BankCardTab />
          <PasswordTab />
        </Tabs.List>
        <PlayerPanel />
        <ContractPanel />
        <BankCardPanel />
        <PasswordPanel />
      </Tabs>
    </Dialog.Modal>
  )
}

const edit = () => "修改"

const title = () => "玩家详情"
