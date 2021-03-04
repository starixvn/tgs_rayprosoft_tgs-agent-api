import React, { useState, useEffect, useMemo } from "react"
import { css, Global } from "@emotion/react"
import { useForm, FormProvider, useFormContext } from "react-hook-form"
import {
    Heading,
    Stack,
    Button,
    Set,
    Text,
    Input,
    Link,
    Dialog,
    Card,
    List
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
import SearchButton from "../../button/SearchButton"
import equals from "fast-deep-equal"
import {
    BrandPaymentStatusSelect,
    ToggleButton as StatusToggleButton
} from "./status"
import { Type as TransferType } from "./transfer"
import { useChangeBrandPaymentOrders, useBrandPaymentList } from "./client"
import { View as ViewMenu, Update as UpdateMenu } from "./menu"
import { Detail as ConditionDetail } from "./condition"
import { EditButton } from "./edit"
import { EditButton as FeeEditButton } from "./handlingfee"
import { ViewButton as LogsButton } from "./logs"
import LoadingBackdrop from "../../dialog/LoadingBackdrop"
import Alert from "../../dialog/Alert"
import { DesignatedPlayersLink } from "./DesignatedPlayersLink"
import { InputField } from "../../input/Input"

export { Code }

export const Content = () => {
    let isDisplay = useIsDisplayMenuByRoute()
    if (!isDisplay) {
        return null
    }

    return <Body />
}

const Body = () => {
    let { state, setState, setPageIndex, setSortOrder } = useTable({
        paymentStatus: null,
        keyword: ""
    })
    let [transferType, setTransferType] = useState(TransferType.Deposit)

    let { data, isLoading, mutate } = useBrandPaymentList({
        ...toApiParams(state),
        transferType,
        sortby: "number"
    })

    let tab = useTabState({
        selectedId: transferType
    })

    const activeTab = (newTransferType) => () => {
        if (transferType === newTransferType) {
            mutate()
        }

        setTransferType(newTransferType)
        setState({
            pageIndex: 0,
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
                        search={(newState) => {
                            if (
                                equals(newState, {
                                    paymentStatus: state.paymentStatus,
                                    keyword: state.keyword
                                })
                            ) {
                                mutate()
                            }
                            setState({ pageIndex: 0, ...newState })
                        }}
                        isLoading={isLoading}
                    />
                    {/* <Set alignX="right" alignY="center">
            <AddVendorButton />
            <AddMethodButton />
            <AddChannelButton />
          </Set> */}
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
                            setPageIndex={setPageIndex}
                            setSortOrder={setSortOrder}
                            transferType={transferType}
                            mutate={mutate}
                            isLoading={isLoading}
                        />
                    )}
                </TabPanel>
                <TabPanel tabId={TransferType.Withdrawal} {...tab}>
                    {transferType === TransferType.Withdrawal && (
                        <DataTable
                            state={state}
                            data={data}
                            setPageIndex={setPageIndex}
                            setSortOrder={setSortOrder}
                            transferType={transferType}
                            mutate={mutate}
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
    setPageIndex,
    setSortOrder,
    transferType,
    mutate,
    isLoading
}) => {
    let { pageSize } = state
    let [displayOrders, setDisplayOrder] = useState({})
    let [submitData, setSubmit] = useState()
    let {
        result,
        isLoading: isChangingDisplayOrder,
        error
    } = useChangeBrandPaymentOrders(submitData)

    const reset = () => {
        setDisplayOrder({})
        setSubmit(undefined)
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
        <Stack spacing="minor-1">
            <Table
                columns={columns}
                data={data?.items}
                pageCount={pageCount(pageSize, data?.totalCount)}
                setPageIndex={setPageIndex}
                setSortOrder={setSortOrder}
                isLoading={isLoading}
            />
            <Dialog.Footer position="sticky" bottom="0">
                <Button
                    isLoading={isChangingDisplayOrder}
                    onClick={() => {
                        let list = Object.entries(displayOrders).map(
                            ([key, value]) => ({
                                id: key,
                                displayOrder: value
                            })
                        )
                        setSubmit({ orders: list })
                    }}
                >
                    <Trans zh="确认排序" en="Confirm Order" />
                </Button>
            </Dialog.Footer>
        </Stack>
    )
}

const SearchBar = ({ search, isLoading, ...props }) => {
    let formMethods = useForm()

    const onSubmit = (data) => search(data)

    return (
        <FormProvider {...formMethods}>
            <form onSubmit={formMethods.handleSubmit(onSubmit)}>
                <Set>
                    <BrandPaymentStatusSelect name="paymentStatus" />
                    <InputField
                        name="keyword"
                        defaultValue=""
                        addonAfter={
                            <Button
                                onClick={() =>
                                    formMethods.setValue("keyword", "")
                                }
                            >
                                X
                            </Button>
                        }
                        placeholder={searchInputPlaceHolder()}
                        autocomplete="off"
                    />
                    <SearchButton isLoading={isLoading} disabled={isLoading} />
                </Set>
            </form>
        </FormProvider>
    )
}

const useColumns = ({
    transferType,
    changeDisplayOrder,
    mutate,
    items = []
}) => {
    let hasUpdateAuth = useIsDisplayMenuByRoute([UpdateMenu])
    let hasViewAuth = useIsDisplayMenuByRoute([ViewMenu])
    let hasEditAuth = useIsDisplayMenuByRoute([UpdateMenu])

    return useMemo(() => {
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
                accessor: "paymentName",
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
                Cell: ({ value }) => <DesignatedPlayersLink record={value} />
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
                    //TODO: format number?
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

        if (hasViewAuth || hasEditAuth) {
            columns.push({
                id: "actions",
                accessor: (row) => row,
                disableSortBy: true,
                Header: <Trans zh="操作" en="Actions" />,
                headCell: {
                    textAlign: "center"
                },
                cell: {
                    textAlign: "center"
                },
                Cell: ({ value }) => {
                    return (
                        <Set alignX="center">
                            <EditButton record={value} onSubmitted={mutate} />
                            <FeeEditButton
                                record={value}
                                onSubmitted={mutate}
                            />
                            <LogsButton record={value} />
                        </Set>
                    )
                }
            })
        }
        return columns
    }, [transferType, JSON.stringify(items)])
}

const searchInputPlaceHolder = () =>
    useTrans({
        zh: "关键字搜索",
        en: "Search..."
    })
