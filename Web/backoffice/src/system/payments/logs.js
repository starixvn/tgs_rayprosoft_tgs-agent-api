import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/core"
import { Formik, Field, Form, useFormikContext } from "formik"
import { Stack, Box, Button, Text, Modal, Dialog } from "bumbag"
import { useModalState } from "../../dialog/Modal"
import { useIsDisplayMenuByRoute } from "../../user/context"
import { View as ViewMenu } from "./menu"
import { Trans, useTrans } from "../../i18n"
import Disclosure from "../../dialog/Disclosure"
import { usePaymentLogs } from "./client"
import LoadingBackdrop from "../../dialog/LoadingBackdrop"
import Alert from "../../dialog/Alert"
import { Table, useTable } from "../../table"
import dateTimeColumn from "../../table/column/dateTime"
import { Detail } from "./condition"

export const ViewButton = ({ record, ...props }) => {
  let dialog = useModalState()

  let isDisplay = useIsDisplayMenuByRoute([ViewMenu])
  if (!isDisplay) {
    return null
  }

  return (
    <>
      <Disclosure {...dialog}>
        <Trans zh="日志" en="Logs" />
      </Disclosure>
      {dialog.visible && <LogsDialog record={record} dialog={dialog} />}
    </>
  )
}

const LogsDialog = ({ record = {}, dialog, ...props }) => {
  let title = useTrans({
    zh: "日志",
    en: "Logs"
  })

  let { data, isLoading, error } = usePaymentLogs(record?.id)
  let items = data?.auditLogs
  let columns = useColumns({ items })

  if (isLoading) {
    return <LoadingBackdrop />
  }

  if (error) {
    return <Alert {...error} onClickClose={dialog.hide} />
  }

  return (
    <Dialog.Modal
      title={title}
      placement="top"
      showCloseButton
      maxWidth="90vw"
      {...dialog}
      css={css`
        & .bb-Flex.bb-DialogContent {
          max-height: 80vh;
        }
      `}
    >
      <Table
        columns={columns}
        data={items}
        isLoading={isLoading}
        usePagination={false}
        maxHeight="65vh"
      />
    </Dialog.Modal>
  )
}

const useColumns = ({ items = [] }) => {
  let columns = [
    {
      id: "cdreatedByUsername",
      accessor: "createdByUsername",
      Header: <Trans zh="操作人" en="Operator" />,
      disableSortBy: true,
      headCell: {
        textAlign: "center"
      },
      cell: {
        textAlign: "center"
      }
    },
    dateTimeColumn({
      accessor: "createdOn",
      Header: <Trans zh="操作时间" en="Operation Time" />,
      disableSortBy: true
    }),
    {
      id: "channelName",
      accessor: (record) => record,
      Header: <Trans zh="通道名称" en="Channel" />,
      disableSortBy: true,
      headCell: {
        textAlign: "center"
      },
      cell: {
        textAlign: "center"
      },
      Cell: ({ value }) => (
        <Stack spacing="minor-4" alignX="center">
          <Text.Block color="textTint" whiteSpace="nowrap">
            {value?.beforeChannelName}
          </Text.Block>
          <Text.Block whiteSpace="nowrap">{value?.afterChannelName}</Text.Block>
        </Stack>
      )
    },
    {
      id: "limit",
      accessor: (record) => record,
      Header: <Trans zh="单笔限额" en="Limit" />,
      disableSortBy: true,
      headCell: {
        textAlign: "center"
      },
      cell: {
        textAlign: "center"
      },
      Cell: ({ value }) => {
        //TODO: format num?
        return (
          <Stack spacing="minor-4" alignX="center">
            <Text.Block color="textTint" whiteSpace="nowrap">
              {value.beforeTransferLimitFrom} - {value.beforeTransferLimitTo}
            </Text.Block>
            <Text.Block whiteSpace="nowrap">
              {value.afterTransferLimitFrom} - {value.afterTransferLimitTo}
            </Text.Block>
          </Stack>
        )
      }
    },
    {
      id: "conditions",
      accessor: (record) => record,
      Header: <Trans zh="开放条件" en="Conditions" />,
      disableSortBy: true,
      headCell: {
        textAlign: "center"
      },
      cell: {
        textAlign: "center"
      },
      Cell: ({ value }) => {
        return (
          <Stack spacing="minor-4" alignX="center">
            <Detail
              record={{
                playerListCondition: value?.beforePlayerListCondition,
                allPlayer: value?.beforeAllPlayers,
                depositedAmountMin: value?.beforeDepositedAmountMin,
                depositedAmountMax: value?.beforeDepositedAmountMax,
                depositedAmountUnlimited: value?.beforeDepositedAmountUnlimited,
                identityAuthentication: value?.beforeIdentityAuthentication,
                phoneAuthentication: value?.beforePhoneAuthentication
                /* TODO: needed?
              vipLevelIds: [],
              */
              }}
              color="textTint"
            />
            <Detail
              record={{
                playerListCondition: value?.afterPlayerListCondition,
                allPlayer: value?.afterAllPlayers,
                depositedAmountMin: value?.afterDepositedAmountMin,
                depositedAmountMax: value?.afterDepositedAmountMax,
                depositedAmountUnlimited: value?.afterDepositedAmountUnlimited,
                identityAuthentication: value?.afterIdentityAuthentication,
                phoneAuthentication: value?.afterPhoneAuthentication
                /* TODO: needed?
              vipLevelIds: [],
              */
              }}
            />
          </Stack>
        )
      }
    },
    {
      accessor: "reason",
      Header: <Trans zh="变更原因" en="Reason" />,
      disableSortBy: true,
      headCell: {
        textAlign: "center"
      },
      cell: {
        textAlign: "center"
      }
    }
  ]

  return useMemo(() => columns, [JSON.stringify(items)])
}
