import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/core"
import { useLocation, useRouteMatch } from "react-router-dom"
import { Formik, Field, Form, useFormikContext } from "formik"
import { useTabState, Tab, TabList, TabPanel } from "reakit/Tab"
import {
  Spinner,
  Stack,
  Flex,
  Box,
  Card,
  Set,
  Button as _Button,
  Text,
  Label,
  Input,
  Checkbox,
  FieldStack,
  FieldWrapper,
  SelectField,
  Tabs as _Tabs,
  Dialog,
  Modal,
  Table,
  useClassName
} from "bumbag"
import { View as ViewMenu } from "../menu"
import { useIsDisplayMenuByRoute } from "../../../user/context"
import {
  Tab as PlayerTab,
  Panel as PlayerPanel,
  Key as PlayerId
} from "./player"
import { Tab as ContractTab, Panel as ContractPanel } from "./contact"
import { Tab as BankTab, Panel as BankPanel } from "./bank"
import { Tab as BettingTab, Panel as BettingPanel } from "./betting"
import { Tab as OperationTab, Panel as OperationPanel } from "./operation"
import LoadingBackdrop from "../../../dialog/LoadingBackdrop"
import AlertDialog from "../../../dialog/Alert"
import { usePlayer } from "../player"

export const Button = ({ player, ...props }) => {
  let dialog = Modal.useState()
  let disclosureRef = useRef()

  let isDisplay = useIsDisplayMenuByRoute([ViewMenu])
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
        {view()}
      </Modal.Disclosure>
      {dialog.visible && (
        <TabsDialog
          playerCode={player.playerCode}
          dialog={dialog}
          finalFocusRef={disclosureRef}
        />
      )}
    </>
  )
}

const TabsDialog = ({ playerCode, dialog, finalFocusRef }) => {
  let tab = useTabState({
    selectedId: PlayerId
  })

  const { player, isLoading, isError, error } = usePlayer({ playerCode })

  return (
    <>
      {isLoading && <LoadingBackdrop />}
      {isError && <AlertDialog {...error} onClickClose={() => dialog.hide()} />}
      {player && (
        <Dialog.Modal
          title={title()}
          showCloseButton={true}
          hideOnClickOutside={false}
          unstable_finalFocusRef={finalFocusRef}
          {...dialog}
          css={css``}
        >
          <_Tabs use={Box} css={css``}>
            <_Tabs.List use={TabList} {...tab}>
              <PlayerTab {...tab} />
              <ContractTab {...tab} />
              <BankTab {...tab} />
              <BettingTab {...tab} />
              <OperationTab {...tab} />
            </_Tabs.List>
            <PlayerPanel player={player} {...tab} />
            <ContractPanel player={player} {...tab} />
            <BankPanel banks={player.banks} {...tab} />
            <BettingPanel playerCode={playerCode} {...tab} />
            <OperationPanel playerCode={playerCode} {...tab} />
          </_Tabs>
        </Dialog.Modal>
      )}
    </>
  )
}

const view = () => "查看"

const title = () => "玩家详情"
