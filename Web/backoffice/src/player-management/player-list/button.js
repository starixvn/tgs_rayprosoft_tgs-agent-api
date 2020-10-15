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
import { View as ViewMenu, Update as UpdateMenu } from "./menu"
import { useIsDisplayMenuByRoute } from "../../user/context"
import {
  Tab as PlayerTab,
  Panel as PlayerPanel,
  EditPanel as PlayerEditTab,
  Key as PlayerId
} from "./player"
import { Tab as ContractTab, Panel as ContractPanel, EditPanel as ContractEditPanel } from "./contact"
import { Tab as BankTab, Panel as BankPanel } from "./bank"
import { Tab as BettingTab, Panel as BettingPanel } from "./betting"
import { Tab as OperationTab, Panel as OperationPanel } from "./operation"
import { Tab as PasswordTab, Panel as PasswordPanel } from "./password"
import LoadingBackdrop from "../../dialog/LoadingBackdrop"
import AlertDialog from "../../dialog/Alert"
import { usePlayer } from "./client"
import { Trans, FormattedDateTime, FormattedNumber, useTrans } from "../../i18n"

export const ViewButton = ({ player, ...props }) => {
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
        <Trans zh="查看" en="View"/>
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

export const EditButton = ({ player, onSubmitted, ...props }) => {
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
        <Trans zh="修改" en="Edit"/>
      </Modal.Disclosure>
      {dialog.visible && 
        <TabsDialog
          edit={true}
          playerCode={player.playerCode}
          onSubmitted={onSubmitted}
          dialog={dialog}
          finalFocusRef={disclosureRef}
        />
      }
      
    </>
  )
}

const TabsDialog = ({ playerCode, edit = false, dialog, finalFocusRef, ...props }) => {
  const readonly = !edit
  let tab = useTabState({
    selectedId: PlayerId
  })

  let title = useTrans({
    zh: "玩家详情",
    en: "Player info"
  })

  const { player, isLoading, isError, error, mutate } = usePlayer({ playerCode })

  const onSubmitted = () => {
    props?.onSubmitted()
    dialog.hide()
    mutate()
  }

  const onBankSubmitted = () => {
    mutate()
  }

  const onClickClose = dialog.hide

  return (
    <>
      {isLoading && <LoadingBackdrop />}
      {isError && <AlertDialog {...error} onClickClose={onClickClose} />}
      {player && (
        <Dialog.Modal
          title={title}
          showCloseButton={true}
          hideOnClickOutside={false}
          unstable_finalFocusRef={finalFocusRef}
          placement="top"
          {...dialog}
          css={css`
            min-width: 80vw;
            min-height: 80vh;

            & .bb-DialogContent {
              min-height: 80vh;
            }
          `}
        >
          {readonly && <ViewTabs playerCode={playerCode} player={player} tab={tab}/>}
          {edit && <EditTabs 
                    playerCode={playerCode} 
                    player={player} 
                    tab={tab} 
                    onSubmitted={onSubmitted} 
                    onBankSubmitted={onBankSubmitted}
                    onClickClose={onClickClose}/>}
        </Dialog.Modal>
      )}
    </>
  )
}

const ViewTabs = ({ playerCode, player, tab }) => {
  return (
    <_Tabs use={Box} isFitted css={css``}>
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
  )
}

const EditTabs = ({ playerCode, player, tab, onSubmitted, onBankSubmitted, onClickClose }) => {
  return (
    <_Tabs use={Box} isFitted css={css``}>
      <_Tabs.List use={TabList} {...tab}>
        <PlayerTab {...tab} />
        <ContractTab {...tab} />
        <BankTab {...tab} />
        <PasswordTab {...tab}/>
      </_Tabs.List>
      <PlayerEditTab player={player} playerCode={playerCode} onSubmitted={onSubmitted} onClickClose={onClickClose} {...tab}/>
      <ContractEditPanel player={player} playerCode={playerCode} onSubmitted={onSubmitted} onClickClose={onClickClose} {...tab} />
      <BankPanel playerCode={playerCode} banks={player.banks} edit={true} onSubmitted={onBankSubmitted} {...tab} />
      <PasswordPanel player={player} {...tab}/>
    </_Tabs>
  )
}