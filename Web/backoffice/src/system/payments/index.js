import React, { useState, useEffect, useMemo } from "react"
import { css, Global } from "@emotion/core"
import { Formik, Field } from "formik"
import { Heading, Stack, Button, InputField, Set, Text } from "bumbag"
import PageContent from "../../page/Content"
import sequenceColumn from "../../table/column/sequence"
import dateTimeColumn from "../../table/column/dateTime"
import {
  useIsDisplayMenuByRoute,
  useHasSubMenuByRoute
} from "../../user/context"
import { Code } from "./menu"
import { useTable, Table, pageCount, toApiParams } from "../../table"
import { Trans, useTrans } from "../../i18n"
import SearchButton from "../../button/Search"
import equals from "fast-deep-equal"
import { Select as StatusSelect, Label as StatusLabel } from "./status"
import { useChangePaymentOrders, usePaymentList } from "./client"
import { View as ViewMenu, Update as UpdateMenu } from "./menu"
import {
  AddButton as AddVendorButton,
  AddMethodButton,
  AddChannelButton
} from "./vendor/edit"

export { Code }

export const Content = () => {
  let isDisplay = useIsDisplayMenuByRoute()
  if (!isDisplay) {
    return null
  }

  return null
  return <Body />
}

const Body = () => {
  let { state, setState, setPageIndex, setSortOrder } = useTable({
    paymentStatus: null,
    keyword: ""
  })
  let { data, isLoading, mutate } = usePaymentList(toApiParams(state))

  return (
    <PageContent
      title={<Trans zh="金流管理" en="Payment Management" />}
      actionBar={
        <>
          <SearchBar
            search={(data) => {
              let newState = {
                ...data,
                pageIndex: 0
              }
              //TODO: need to compare state value?
              mutate()
              setState(newState)
            }}
            isLoading={isLoading}
          />
          <Set alignX="right" alignY="center">
            TODO: remove add buttons
            <AddVendorButton />
            <AddMethodButton />
            <AddChannelButton />
          </Set>
        </>
      }
    >
      <DataTable
        state={state}
        data={data}
        mutate={mutate}
        setPageIndex={setPageIndex}
        setSortOrder={setSortOrder}
        isLoading={isLoading}
      />
    </PageContent>
  )
}

export const CodeContentPair = [Code, Content]

const DataTable = ({
  state,
  data,
  mutate,
  setPageIndex,
  setSortOrder,
  isLoading
}) => {
  let { pageSize } = state
  let [displayOrders, setDisplayOrder] = useState({})
  let [submitData, setSubmit] = useState()
  let {
    result,
    isLoading: isChangingDisplayOrder,
    error
  } = useChangePaymentOrders(submitData)

  const reset = () => {
    setDisplayOrder({})
    setSubmit(null)
  }

  useEffect(() => {
    if (result && !error) {
      reset()
      mutate()
    }
  }, [result, error])

  const changeDisplayOrder = ({ id, value }) => {
    let newDisplayOrders = {
      ...displayOrders
    }
    Reflect.set(newDisplayOrders, id, value)
    setDisplayOrder(newDisplayOrders)
  }

  let columns = useColumns({
    mutate,
    changeDisplayOrder,
    items: data?.items ?? []
  })
  return (
    <Table
      controlledState={state}
      columns={columns}
      data={data?.items}
      pageCount={pageCount(pageSize, data?.totalCount)}
      setPageIndex={setPageIndex}
      setSortOrder={setSortOrder}
      isLoading={isLoading}
    />
  )
}

const SearchBar = ({ paymentStatus, search, isLoading, ...props }) => {
  let [status, setStatus] = useState(paymentStatus)

  return (
    <Set>
      <StatusSelect value={status} onChange={(value) => setStatus(value)} />
      <SearchButton
        isLoading={isLoading}
        onClick={(event) => {
          search({
            paymentStatus: status
          })
        }}
      />
    </Set>
  )
}

const useColumns = ({ changeDisplayOrder, mutate, items = [] }) => {
  let columns = [
    {
      id: "displayOrder",
      accessor: (record) => record,
      Header: <Trans zh="序" en="Order" />,
      disableSortBy: true,
      headCell: {
        textAlign: "center"
      },
      cell: {
        textAlign: "center"
      },
      Cell: ({ value }) => {
        let { id, displayOrder = "" } = value
        return (
          <Input
            type="number"
            size="small"
            min="0"
            minLength="1"
            width="5em"
            defaultValue={displayOrder}
            onChange={(event) => {
              let value = event.target.value
              let number = parseInt(value)
              if (Number.isFinite(number)) {
                changeDisplayOrder({
                  id,
                  value: number
                })
              }
            }}
          />
        )
      }
    },
    {
      accessor: "paymentVendorName",
      Header: <Trans zh="支付厂商" en="Payment Vendor" />,
      disableSortBy: true,
      headCell: {
        textAlign: "center"
      },
      cell: {
        textAlign: "center"
      }
    },
    {
      accessor: "paymentMethodName",
      Header: <Trans zh="支付方式" en="Payment Method" />,
      disableSortBy: true,
      headCell: {
        textAlign: "center"
      },
      cell: {
        textAlign: "center"
      }
    },
    {
      accessor: "paymentChannelName",
      Header: <Trans zh="支付通道" en="Payment Channel" />,
      disableSortBy: true,
      headCell: {
        textAlign: "center"
      },
      cell: {
        textAlign: "center"
      }
    },
    {
      accessor: "channelName",
      Header: <Trans zh="通道名称" en="Channel Name" />,
      disableSortBy: true,
      headCell: {
        textAlign: "center"
      },
      cell: {
        textAlign: "center"
      }
    },
    {
      id: "target",
      accessor: (record) => record,
      Header: <Trans zh="开放对象" en="Target" />,
      disableSortBy: true,
      headCell: {
        textAlign: "center"
      },
      cell: {
        textAlign: "center"
      },
      Cell: ({ value }) => {
        return <Box>TODO</Box>
      }
    },
    {
      id: "designatedUsers",
      accessor: (record) => record,
      Header: <Trans zh="特定用户" en="Designated Users" />,
      disableSortBy: true,
      headCell: {
        textAlign: "center"
      },
      cell: {
        textAlign: "center"
      },
      Cell: ({ value }) => {
        return <Box>TODO</Box>
      }
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
        return <Box>TODO</Box>
      }
    },
    {
      accessor: "paymentStatus",
      Header: <Trans zh="通道名称" en="Channel Name" />,
      disableSortBy: true,
      headCell: {
        textAlign: "center"
      },
      cell: {
        textAlign: "center"
      },
      Cell: ({ value }) => {
        return (
          <Text.Block
            css={css`
              white-space: nowrap;
            `}
          >
            <StatusLabel value={value} />
          </Text.Block>
        )
      }
    }
  ]

  let hasViewAuth = useIsDisplayMenuByRoute([ViewMenu])
  let hasEditAuth = useIsDisplayMenuByRoute([UpdateMenu])
  if (hasViewAuth || hasEditAuth) {
    columns.push({
      id: "actions",
      disableSortBy: true,
      Header: <Trans zh="操作" en="Actions" />,
      headCell: {
        textAlign: "center"
      },
      cell: {
        textAlign: "center"
      },
      accessor: (row) => {
        return row
      },
      Cell: ({ value }) => {
        return (
          <Box alignX="center">
            <Set>TODO: buttons</Set>
          </Box>
        )
      }
    })
  }
  return useMemo(() => columns, [changeDisplayOrder, JSON.stringify(items)])
}
