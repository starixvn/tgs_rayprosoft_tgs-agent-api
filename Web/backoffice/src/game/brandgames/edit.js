import React, { useState, useEffect, useMemo } from "react"
import {
  css,
  Stack,
  Heading,
  Box,
  Button,
  Text,
  Group,
  Set,
  Flex,
  Switch
} from "bumbag"
import { useModalState } from "../../dialog/Modal"
import { useIsDisplayMenuByRoute } from "../../user/context"
import { Update as UpdateMenu } from "../providers/menu"
import { EditDisclosure } from "../../dialog/Disclosure"
import { ConfirmButtonFooter } from "../../form/Dialog"
import { useUpdateBrandGame } from "./client"
import { Trans, useTrans } from "../../i18n"
import { FieldValues } from "../../layout/Grid"
import Label from "../../form/Label"
import HookFormDialog from "../../form/HookFormDialog"
import { GameTypeLabel } from "../games/GameType"
import { GamePlatformLabel } from "../games/GamePlatform"
import { useFormContext } from "react-hook-form"
import LoadingBackdrop from "../../dialog/LoadingBackdrop"
import { useBrandGame } from "./client"
import AlertDialog from "../../dialog/Alert"
import HookFormInputField from "../../input/InputField"

export const EditButton = ({ record, onSubmitted, ...props }) => {
  let dialog = useModalState()
  let isDisplay = useIsDisplayMenuByRoute([UpdateMenu])
  if (!isDisplay) {
    return null
  }

  return (
    <>
      <EditDisclosure {...dialog} />
      {dialog.visible && (
        <EditDialog record={record} onSubmitted={onSubmitted} dialog={dialog} />
      )}
    </>
  )
}

const EditDialog = ({ record = {}, dialog, ...props }) => {
  let { id } = record
  let {
    data: brandGame,
    isLoading: isLoadingBrandGame,
    error: brandGameError,
    mutate: mutatebrandGame,
    deleteCache: deleteBrandGameCache
  } = useBrandGame(id)

  let [submitData, setSubmit] = useState()
  let { result, isLoading, error, mutate, deleteCache } = useUpdateBrandGame(
    record?.id,
    submitData
  )

  const onSubmit = (formData) => {
    setSubmit(formData)
  }

  const reset = () => {
    mutatebrandGame(undefined, false)
    deleteBrandGameCache()

    mutate(undefined, false)
    setSubmit(undefined)
  }

  let title = useTrans({
    zh: "编辑",
    en: "Edit"
  })

  if (isLoadingBrandGame) {
    return <LoadingBackdrop {...dialog} />
  }

  if (brandGameError) {
    return <AlertDialog {...brandGameError} onClickClose={dialog.hide} />
  }

  return (
    <HookFormDialog
      defaultValues={{ ...brandGame }}
      onSubmit={onSubmit}
      result={result}
      isLoading={isLoadingBrandGame}
      error={error}
      onCloseErrorAlert={reset}
      onSubmitted={() => {
        reset()
        props?.onSubmitted()
      }}
      title={title}
      showCloseButton
      showActionButtons={false}
      dialog={dialog}
      footer={
        <ConfirmButtonFooter isLoading={isLoading} disabled={isLoading} />
      }
      maxWidth="unset"
    >
      <EditBody record={brandGame} />
    </HookFormDialog>
  )
}

const EditBody = ({ record = {} }) => {
  let {
    gameProviderName,
    gameType,
    gameName,
    gameId,
    platform,
    supportHtml5
  } = record
  let { register, watch } = useFormContext()
  return (
    <Stack orientation="vertical" width="500px">
      <Heading use="h5" marginBottom="major-1">
        <Trans zh="基本资料" en="Basic Information" />
      </Heading>
      <Stack>
        <Stack orientation="horizontal" marginBottom="major-1">
          <FieldValues>
            <Label fontWeight="normal">
              <Trans zh="厂商" en="Vendor" />
            </Label>
            <ValueBlock>{gameProviderName}</ValueBlock>
          </FieldValues>

          <FieldValues>
            <Label fontWeight="normal">
              <Trans zh="游戏类型" en="Game Type" />
            </Label>
            <ValueBlock>
              <GameTypeLabel value={gameType} />
            </ValueBlock>
          </FieldValues>
        </Stack>
        <Stack orientation="horizontal" marginBottom="major-1">
          <FieldValues>
            <Label fontWeight="normal">
              <Trans zh="游戏名称" en="Game Name" />
            </Label>
            <ValueBlock>{gameName}</ValueBlock>
          </FieldValues>
          <FieldValues>
            <Label fontWeight="normal">
              <Trans zh="游戏 ID" en="Game ID" />
            </Label>
            <ValueBlock>{gameId}</ValueBlock>
          </FieldValues>
        </Stack>
        <Stack orientation="horizontal" marginBottom="major-1">
          <FieldValues>
            <Label fontWeight="normal">
              <Trans zh="支持设备" en="Platform" />
            </Label>
            <ValueBlock>
              <GamePlatformLabel value={platform} />
            </ValueBlock>
          </FieldValues>

          <FieldValues>
            <Label fontWeight="normal">
              <Trans zh="支持HTML5" en="HTML5" />
            </Label>
            <ValueBlock>
              {supportHtml5 ? (
                <Trans zh="是" en="Yes" />
              ) : (
                <Trans zh="不" en="No" />
              )}
            </ValueBlock>
          </FieldValues>
        </Stack>
      </Stack>

      <Heading use="h5" marginBottom="major-1">
        <Trans zh="特殊处理" en="Special Processing" />
      </Heading>
      <FieldValues>
        <Label fontWeight="normal">
          <Trans zh="有效投注比例" en="Effective Betting Proportion" />
        </Label>
        <ValueBlock>
          <HookFormInputField
            name="effectiveBettingPercentage"
            type="number"
            registerOptions={{
              valueAsNumber: true
            }}
            step="0.01"
            min="0"
            max="100"
            addonAfter={<Button isStatic>%</Button>}
          />
        </ValueBlock>

        <Label fontWeight="normal">
          <Trans zh="是否发放返水" en="Rebate" />
        </Label>
        <ValueBlock>
          <Switch name="rebate" switchRef={register} />
        </ValueBlock>
      </FieldValues>
    </Stack>
  )
}

const ValueBlock = ({ children, ...props }) => (
  <Text.Block alignY="center" {...props}>
    {children}
  </Text.Block>
)
