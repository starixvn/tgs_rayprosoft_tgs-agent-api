import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/react"
import { useModalState, Stack, Heading, Box, Text, FieldStack } from "bumbag"
import { useIsDisplayMenuByRoute, useUserLicenseeId } from "../../user/context"
import Disclosure from "../../dialog/Disclosure"
import FormDialog from "../../form/FormDialog"
import { Update as UpdateMenu } from "./menu"
import { Trans, useTrans } from "../../i18n"
import LoadingBackdrop from "../../dialog/LoadingBackdrop"
import AlertDialog from "../../dialog/Alert"
import {
  CloseButtonFooter,
  ConfirmButtonFooter
} from "../../form/ConfirmButtonFooter"
import Input from "../../input/Input"
import { GameProviderCheckboxGroup } from "../../game/providers/GameProviderCheckboxGroup"
import {
  mapChecked,
  mapCheckedById,
  mapCheckedByCultureCode,
  mapCheckedByCode
} from "../../checkbox/mapChecked"
import { LanguageCheckboxGroupTable } from "../../language/LanguageCheckboxGroup"
import { CurrencyCheckboxGroupTable } from "../../currency/CurrencyCheckboxGroup"
import { BetLimitCheckboxGroup } from "../../betlimit/BetLimitCheckboxGroup"
import { allCheckedKeys } from "../../checkbox/allCheckedKeys"
import { MerchantIdSelect } from "../MerchantSelect"
import { RoleTypes } from "../../system/roles/type"
import { useBrand } from "./client"
import { useLicensee } from "../licensees/client"
import { useFormContext } from "react-hook-form"
import { FieldValues } from "../../layout/Grid"
import { NormalLabel } from "../../form/Label"
import Textarea from "../../textarea"

export const AddButton = ({ onSubmitted, ...props }) => {
  let dialog = useModalState()

  let isDisplay = useIsDisplayMenuByRoute([UpdateMenu])
  if (!isDisplay) {
    return null
  }

  return (
    <>
      <Disclosure {...dialog}>
        <Trans zh="新增" en="Add" />
      </Disclosure>
      {dialog.visible && (
        <AddDialog onSubmitted={onSubmitted} dialog={dialog} />
      )}
    </>
  )
}

const AddDialog = ({ onSubmitted, dialog }) => {
  let title = useTrans({
    zh: "新增品牌",
    en: "Add Brand"
  })
  return (
    <FormDialog title={title} dialog={dialog}>
      <EditBody />
    </FormDialog>
  )
}

export const ViewButton = ({ record, ...props }) => {
  let dialog = useModalState()

  /* TODO: view auth?
  let isDisplay = useIsDisplayMenuByRoute([ViewMenu])
  if (!isDisplay) {
    return null
  }
  */
  return (
    <>
      <Disclosure {...dialog} {...props}>
        <Trans zh="查看" en="View" />
      </Disclosure>
      {dialog.visible && <ViewDialog record={record} dialog={dialog} />}
    </>
  )
}

const useLicenseeBrand = ({ id = "", licenseeId = "" }) => {
  let {
    data: brand,
    isLoading: isLoadingBrand,
    error: brandError,
    mutate: mutateBrand,
    deleteCache: deleteBrandCache
  } = useBrand(id)

  let {
    data: licensee,
    isLoading: isLoadingLicensee,
    error: licenseeError,
    mutate: mutateLicensee,
    deleteCache: deleteLicenseeCache
  } = useLicensee(licenseeId)

  return {
    licensee,
    brand,
    languages: mapCheckedByCultureCode(
      brand?.languages ?? [],
      licensee?.languages ?? []
    ),
    currencies: mapCheckedByCode(
      brand?.currencies ?? [],
      licensee?.currencies ?? []
    ),
    gameProviders: mapCheckedById(
      brand?.gameProviders ?? [],
      licensee?.gameProviders ?? []
    ),
    betLimits: mapCheckedById(
      brand?.betLimits ?? [],
      licensee?.betLimits ?? []
    ),
    isLoading: isLoadingBrand || isLoadingLicensee,
    error: brandError || licenseeError,
    mutateBrand,
    deleteBrandCache
  }
}

const ViewDialog = ({ record, dialog, ...props }) => {
  let { id, licenseeId } = record
  let {
    brand,
    languages,
    currencies,
    gameProviders,
    betLimits,
    isLoading,
    error,
    mutateBrand,
    deleteBrandCache
  } = useLicenseeBrand({ id, licenseeId })

  let title = useTrans({
    zh: "查看品牌",
    en: "View Brand"
  })

  if (isLoading) {
    return <LoadingBackdrop {...dialog} onClickClose={dialog.hide} />
  }

  if (error) {
    return <AlertDialog {...error} {...dialog} onClickClose={dialog.hide} />
  }

  const reset = () => {
    mutateBrand(undefined, false)
    deleteBrandCache()
  }

  return (
    <FormDialog
      defaultValues={{
        ...brand,
        languages,
        currencies,
        gameProviders,
        betLimits
      }}
      onClickClose={reset}
      title={title}
      dialog={dialog}
      showActionButtons={false}
      onClickClose={reset}
      footer={<CloseButtonFooter dialog={dialog} onClickClose={reset} />}
      placement="top"
      minWidth="60vw"
      maxWidth="unset"
      css={css`
        & .bb-DialogContent {
          position: relative;
        }
      `}
    >
      <EditBody readonly record={record} />
    </FormDialog>
  )
}

export const EditButton = ({ record, onSubmitted, ...props }) => {
  let dialog = useModalState()

  let isDisplay = useIsDisplayMenuByRoute([UpdateMenu])
  if (!isDisplay) {
    return null
  }

  return (
    <>
      <Disclosure {...dialog}>
        <Trans zh="修改" en="Edit" />
      </Disclosure>
      {dialog.visible && (
        <EditDialog record={record} onSubmitted={onSubmitted} dialog={dialog} />
      )}
    </>
  )
}

const EditDialog = ({ record, onSubmitted, dialog, ...props }) => {
  let title = useTrans({
    zh: "编辑品牌",
    en: "Edit Brand"
  })

  return (
    <FormDialog title={title} dialog={dialog}>
      <EditBody />
    </FormDialog>
  )
}

const EditBody = ({ record, readonly, isAdd = false, isLoadingLicensee }) => {
  let { watch, getValues, register } = useFormContext()

  let userLicenseeId = useUserLicenseeId()
  let licensee = watch("licensee")
  let languages = watch("languages")
  let currencies = watch("currencies")
  let gameProviders = watch("gameProviders")
  let betLimits = watch("betLimits")

  let editable = !readonly
  return (
    <Stack orientation="vertical">
      <Heading use="h5" marginBottom="major-1">
        <Trans zh="基本资料" en="Basic Information" />
      </Heading>

      <FieldValues>
        <NormalLabel>
          <Trans zh="所属公司" en="Licensee" />
        </NormalLabel>
        <Box>
          {!userLicenseeId && isAdd && (
            <MerchantIdSelect end={RoleTypes.Licensee} />
          )}
          {!isAdd && <Text.Block>{getValues("licensee")}</Text.Block>}
        </Box>

        <NormalLabel>
          <Trans zh="品牌代码" en="Code" />
        </NormalLabel>
        <Box alignY="center">
          <Input name="code" type={isAdd ? "input" : "hidden"} />
          {!isAdd && <Text.Block>{getValues("code")}</Text.Block>}
        </Box>

        <NormalLabel>
          <Trans zh="品牌名称" en="Name" />
        </NormalLabel>
        <Box alignY="center">
          {editable && <Input name="name" required />}
          {readonly && <Text.Block>{record?.name}</Text.Block>}
        </Box>

        <NormalLabel>
          <Trans zh="品牌简介" en="Description" />
        </NormalLabel>
        <Box alignY="center">
          {editable && <Textarea name="description" />}
          {readonly && <Text.Block>{record.description}</Text.Block>}
        </Box>

        <NormalLabel>
          <Trans zh="品牌邮箱" en="Email" />
        </NormalLabel>
        <Box alignY="center">
          {editable && <Input type="email" name="operationalEmail" required />}
          {readonly && <Text.Block>{record.operationalEmail}</Text.Block>}
        </Box>

        {languages && (
          <>
            <NormalLabel>
              <Trans zh="语言" en="Language" />
            </NormalLabel>
            <LanguageCheckboxGroupTable disabled={readonly} />
          </>
        )}

        {currencies && (
          <>
            <NormalLabel>
              <Trans zh="币种" en="Currency" />
            </NormalLabel>
            <CurrencyCheckboxGroupTable disabled={readonly} />
          </>
        )}

        {gameProviders && (
          <>
            <NormalLabel>
              <Trans zh="游戏厂商" en="Game Provider" />
            </NormalLabel>
            <GameProviderCheckboxGroup
              name="gameProviderIds"
              fieldArrayName="gameProviders"
              marginTop="major-1"
              disabled={readonly}
            />
          </>
        )}

        {betLimits && (
          <>
            <NormalLabel>
              <Trans zh="游戏商限红" en="Game Provider Bet Limit" />
            </NormalLabel>
            <FieldStack marginTop="major-1">
              <BetLimitCheckboxGroup disabled={readonly} />
            </FieldStack>
          </>
        )}
      </FieldValues>
    </Stack>
  )
}
