import React, { useState, useEffect, useMemo } from "react"
import { css, Global } from "@emotion/core"
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
  Card
} from "bumbag"
import PageContent from "../../page/Content"
import sequenceColumn from "../../table/column/sequence"
import dateTimeColumn from "../../table/column/dateTime"
import {
  useIsDisplayMenuByRoute,
  useHasSubMenuByRoute
} from "../../user/context"
import { Code } from "./menu"
import { useTabState, Tabs, TabList, Tab, TabPanel } from "../../tab"
import { useTable, Table, pageCount, toApiParams } from "../../table"
import { Trans, useTrans } from "../../i18n"
import SearchButton from "../../button/Search"
import equals from "fast-deep-equal"
import {
  Field as StatusSelectField,
  Label as StatusLabel,
  from as toStatus,
  ToggleButton as StatusToggleButton
} from "./status"
import { Type as TransferType, from as transferTypeFrom } from "./transfer"
import { useChangePaymentOrders, usePaymentList } from "./client"
import { View as ViewMenu, Update as UpdateMenu } from "./menu"
import {
  AddButton as AddVendorButton,
  AddMethodButton,
  AddChannelButton
} from "./vendor/edit"
import Disclosure from "../../dialog/Disclosure"
import { Modal, useModalState } from "../../dialog/Modal"
import { CloseText } from "../../button/ActionButtons"
import { Detail as ConditionDetail } from "./condition"

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
    transferType: TransferType.Deposit,
    paymentStatus: null,
    keyword: ""
  })

  let { transferType } = state
  let { data, isLoading, mutate } = usePaymentList(toApiParams(state))

  let tab = useTabState({
    selectedId: transferType
  })

  const activeTab = (newTransferType) => () => {
    if (transferType === newTransferType) {
      mutate()
    }
    setState({
      pageIndex: 0,
      transferType: newTransferType,
      paymentStatus: null,
      keyword: ""
    })
  }

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
              mutate()
              setState(newState)
            }}
            isLoading={isLoading}
          />
          <Set alignX="right" alignY="center">
            <AddVendorButton />
            <AddMethodButton />
            <AddChannelButton />
          </Set>
        </>
      }
    >
      <Tabs>
        <TabList>
          <Tab
            tabId={TransferType.Deposit}
            onClick={activeTab(TransferType.Deposit)}
            {...tab}
          >
            <Trans zh="存款" en="Deposit" />
          </Tab>
          <Tab
            tabId={TransferType.Withdrawal}
            onClick={activeTab(TransferType.Withdrawal)}
            {...tab}
          >
            <Trans zh="提款" en="Withdrawal" />
          </Tab>
        </TabList>
        <TabPanel tabId={TransferType.Deposit} {...tab}>
          {transferType === TransferType.Deposit && (
            <DataTable
              state={state}
              data={data}
              transferType={transferType}
              mutate={mutate}
              setPageIndex={setPageIndex}
              setSortOrder={setSortOrder}
              isLoading={isLoading}
            />
          )}
        </TabPanel>
        <TabPanel tabId={TransferType.Withdrawal} {...tab}>
          {transferType === TransferType.Withdrawal && (
            <DataTable
              state={state}
              data={data}
              transferType={transferType}
              mutate={mutate}
              setPageIndex={setPageIndex}
              setSortOrder={setSortOrder}
              isLoading={isLoading}
            />
          )}
        </TabPanel>
      </Tabs>
    </PageContent>
  )
}

export const CodeContentPair = [Code, Content]

const DataTable = ({
  state,
  data,
  transferType,
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
    transferType,
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

const SearchBar = ({ search, isLoading, ...props }) => {
  return (
    <Formik
      initialValues={{
        paymentStatus: "",
        keyword: ""
      }}
      onSubmit={search}
    >
      {(formProps) => (
        <form onReset={formProps.handleReset} onSubmit={formProps.handleSubmit}>
          <Set>
            <StatusSelectField name="paymentStatus" />
            <Field
              name="keyword"
              component={InputField.Formik}
              addonAfter={
                <Button onClick={() => formProps.setFieldValue("keyword", "")}>
                  X
                </Button>
              }
              placeholder={searchInputPlaceHolder()}
              autocomplete="off"
            />
            <SearchButton isLoading={isLoading} />
          </Set>
        </form>
      )}
    </Formik>
  )
}

const useColumns = ({
  transferType,
  changeDisplayOrder,
  mutate,
  items = []
}) => {
  let hasUpdateAuth = useIsDisplayMenuByRoute([UpdateMenu])
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
        let { id, number: displayOrder = "" } = value
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
        textAlign: "center",
        whiteSpace: "nowrap"
      },
      cell: {
        textAlign: "center",
        whiteSpace: "nowrap"
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
        textAlign: "center",
        whiteSpace: "nowrap"
      },
      cell: {
        textAlign: "center",
        whiteSpace: "nowrap"
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
        return <ConditionDetail record={value} />
      }
    },
    {
      id: "designatedUsers",
      accessor: (record) => record,
      Header: <Trans zh="特定用户" en="Designated Users" />,
      disableSortBy: true,
      headCell: {
        textAlign: "center",
        whiteSpace: "nowrap"
      },
      cell: {
        textAlign: "center",
        whiteSpace: "nowrap"
      },
      Cell: ({ value }) => {
        let { allPlayer = true, designatedUsers = [] } = value
        if (allPlayer) {
          return <Trans zh="无" en="None" />
        }

        return <DesignatedUsersLink users={designatedUsers} />
      }
    },
    {
      id: "limit",
      accessor: (record) => record,
      Header: <Trans zh="单笔限额" en="Limit" />,
      disableSortBy: true,
      headCell: {
        textAlign: "center",
        whiteSpace: "nowrap"
      },
      cell: {
        textAlign: "center",
        whiteSpace: "nowrap"
      },
      Cell: ({ value }) => {
        //TODO: 0 - Unlimited?
        //TODO: format num?
        if (transferType === TransferType.Deposit) {
          return (
            <Text.Block whiteSpace="nowrap">
              {value.depositedAmountFrom} - {value.depositedAmountTo}
            </Text.Block>
          )
        }

        return (
          <Text.Block whiteSpace="nowrap">
            {value.transferLimitFrom} - {value.transferLimitTo}
          </Text.Block>
        )
      }
    },
    {
      id: "paymentStatus",
      accessor: (record) => record,
      Header: <Trans zh="状态" en="Status" />,
      disableSortBy: true,
      headCell: {
        textAlign: "center"
      },
      cell: {
        textAlign: "center"
      },
      Cell: ({ value }) => {
        return (
          <StatusToggleButton
            record={value}
            disabled={!hasUpdateAuth}
            onSubmitted={mutate}
          />
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
  return useMemo(() => columns, [transferType, JSON.stringify(items)])
}

const searchInputPlaceHolder = () =>
  useTrans({
    zh: "关键字搜索",
    en: "Search..."
  })

const DesignatedUsersLink = ({ users = [] }) => {
  let dialog = useModalState()

  let title = useTrans({
    zh: "特定用户",
    en: "Designated Users"
  })
  return (
    <>
      <Disclosure use={Link} {...dialog}>
        <Trans zh="有" en="Users" />
      </Disclosure>
      {dialog.visible && (
        <Dialog.Modal
          title={title}
          footer={
            <Box alignX="right" width="100%">
              <Button onClick={dialog.hide}>
                <CloseText />
              </Button>
            </Box>
          }
          {...dialog}
        >
          <Card variant="bordered">
            <Set orientation="vertical" spacing="major-1">
              {users.map((user) => (
                <Text.Block>{user}</Text.Block>
              ))}
            </Set>
          </Card>
        </Dialog.Modal>
      )}
    </>
  )
}
